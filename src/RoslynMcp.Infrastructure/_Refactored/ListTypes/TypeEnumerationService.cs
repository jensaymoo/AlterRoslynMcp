using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace RoslynMcp.Infrastructure._Refactored;

public class TypeEnumerationService(
    ILogger<TypeEnumerationService> logger,
    ISolutionWorkspaceService solutionWorkspaceService): ITypeEnumerationService
{
  public async Task<IEnumerable<TypeEntry>> EnumerateTypesBySolutionAsync(bool includeSummary, CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            var allTypes = new List<TypeEntry>();
            
            foreach (var project in solution.Projects)
            {
                if (await project.GetCompilationAsync(ct) is { } compilation)
                {
                    var projectTrees = await GetProjectSyntaxTreesAsync(project, ct);
                    
                    var types = compilation.GlobalNamespace.EnumerateTypes(includeGenerated: false)
                          .Where(type => type.DeclaringSyntaxReferences
                              .Select(r => r.SyntaxTree).Any(projectTrees.Contains));

                    var typesWithBaseTypes = new List<TypeEntry>();
                    foreach (var type in types)
                    {
                        var directBaseTypes = await GetDirectBaseTypesAsync(type, compilation, project, ct);
                        
                        typesWithBaseTypes.Add(CreateTypeEntry(
                            type, 
                            project, 
                            includeSummary ? type.GetDocumentationCommentXml() : null
                        ));
}
                    
                    allTypes.AddRange(typesWithBaseTypes);
                }
            }
            
            return OrderTypes(allTypes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during type enumeration");
            throw;
        }
    }
    
    private static TypeEntryKind GetTypeEntryKind(INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord)
            return TypeEntryKind.Record;

        return symbol.TypeKind switch
        {
            TypeKind.Class => TypeEntryKind.Class,
            TypeKind.Interface => TypeEntryKind.Interface,
            TypeKind.Enum => TypeEntryKind.Enum,
            TypeKind.Struct => TypeEntryKind.Struct,
            _ => TypeEntryKind.Unknown
        };
    }

    private static SymbolAccessibility GetTypeEntryAccessibility(INamedTypeSymbol symbol)
    {
        return symbol.DeclaredAccessibility switch
        {
            Microsoft.CodeAnalysis.Accessibility.Internal => SymbolAccessibility.Internal,
            Microsoft.CodeAnalysis.Accessibility.Private => SymbolAccessibility.Private,
            Microsoft.CodeAnalysis.Accessibility.Protected => SymbolAccessibility.Protected,
            Microsoft.CodeAnalysis.Accessibility.Public => SymbolAccessibility.Public,
            Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => SymbolAccessibility.PrivateProtected,
            Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal => SymbolAccessibility.ProtectedInternal,
            _ => SymbolAccessibility.NotApplicable
        };
    }

    private IEnumerable<TypeEntry> OrderTypes(IEnumerable<TypeEntry> entries)
        => entries
            .OrderBy(item => item.SymbolName, StringComparer.Ordinal)
            .ToArray();

    private async Task<IEnumerable<SyntaxTree>> GetProjectSyntaxTreesAsync(Project project, CancellationToken ct)
        => await Task.WhenAll(
            project.Documents
                .Where(d => d.SupportsSyntaxTree)
                .Select(d => d.GetSyntaxTreeAsync(ct))
        ) ?? Array.Empty<SyntaxTree>();

    private TypeEntry CreateTypeEntry(INamedTypeSymbol symbol, Project project, string? summary)
    {
        return new TypeEntry
        {
            Accessibility = GetTypeEntryAccessibility(symbol),
            Kind = GetTypeEntryKind(symbol),
            SymbolName = symbol.Name,
            Namespace = symbol.ContainingNamespace.IsGlobalNamespace 
                ? null 
                : symbol.ContainingNamespace.ToDisplayString(),
            Location = symbol.Locations.AsSourceLocations(),
            Summary = summary,
            ProjectName = project.Name,
            ProjectPath = project.FilePath,
            BaseTypes = null
        };
    }

    private async Task<IEnumerable<TypeEntry>?> GetDirectBaseTypesAsync(INamedTypeSymbol type, Compilation compilation, Project project, CancellationToken ct)
    {
        if (type.TypeKind == TypeKind.Enum)
        {
            return null;
        }

        var directBaseTypes = new List<TypeEntry>();
        var typesToLookup = new List<INamedTypeSymbol>();

        if (type.BaseType != null && !type.BaseType.IsImplicitlyDeclared)
        {
            typesToLookup.Add(type.BaseType);
        }

        foreach (var iface in type.Interfaces)
        {
            if (!iface.IsImplicitlyDeclared)
            {
                typesToLookup.Add(iface);
            }
        }

        if (typesToLookup.Count == 0)
        {
            return null;
        }

        var entries = await Task.WhenAll(
            typesToLookup.Select(t => CreateTypeEntryOrNullAsync(t, compilation, project, ct))
        );

        directBaseTypes.AddRange(entries.OfType<TypeEntry>());

        return directBaseTypes.Count > 0 ? directBaseTypes : null;
    }

    private async Task<TypeEntry?> CreateTypeEntryOrNullAsync(INamedTypeSymbol symbol, Compilation compilation, Project project, CancellationToken ct)
    {
        if (!symbol.DeclaringSyntaxReferences.Any())
        {
            return null;
        }

        var syntaxTree = symbol.DeclaringSyntaxReferences.First().SyntaxTree;
        var projectTrees = await GetProjectSyntaxTreesAsync(project, ct);
        var isFromThisProject = projectTrees.Contains(syntaxTree);
        
        if (!isFromThisProject)
        {
            return null;
        }

        return CreateTypeEntry(symbol, project, null);
    }

  }