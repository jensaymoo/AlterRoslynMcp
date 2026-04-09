using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public enum TypeEntryKind
{
    Record,
    Class,
    Struct,
    Enum,
    Interface,
    Unknown
}

public enum TypeEntryAccessibility
{
    Public,
    Internal,
    Protected,
    Private,
    ProtectedInternal,
    PrivateProtected,
    NotApplicable
}

public sealed class TypeEntry
{
    public required string DisplayName { get; init; }
    public required string Namespace { get; init; }
    public required IEnumerable<SourceLocation?> Location { get; init; }
    
    public required TypeEntryAccessibility Accessibility { get; init; }
    public required TypeEntryKind Kind { get; init; }
    public required string? Summary { get; init; }
    
    public required string ProjectName { get; init; }
    public required string? ProjectPath { get; init; }
}

public class TypeEnumerationService(
    ILogger<TypeEnumerationService> logger,
    ITypeResolverService typeResolverService,
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
                    var projectTrees = await Task.WhenAll(
                        project.Documents
                            .Where(d => d.SupportsSyntaxTree)
                            .Select(d => d.GetSyntaxTreeAsync(ct))
                    );
                    
                    var types = compilation.GlobalNamespace.EnumerateTypes(includeGenerated: false)
                        .Where(type => type.DeclaringSyntaxReferences
                            .Select(r => r.SyntaxTree).Any(projectTrees.Contains))
                        .Select(type => new TypeEntry
                        {
                            Accessibility = GetTypeEntryAccessibility(type),
                            Kind = GetTypeEntryKind(type),
                            DisplayName = typeResolverService.GetDisplayName(type),
                            Namespace = typeResolverService.GetDisplayNamespace(type),
                            Location = type.Locations.AsSourceLocations(),
                            Summary = includeSummary ? type.GetDocumentationCommentXml() : null,
                            ProjectName = project.Name,
                            ProjectPath = project.FilePath,
                        });
                    
                    allTypes.AddRange(types);
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

    private static TypeEntryAccessibility GetTypeEntryAccessibility(INamedTypeSymbol symbol)
    {
        return symbol.DeclaredAccessibility switch
        {
            Accessibility.Internal => TypeEntryAccessibility.Internal,
            Accessibility.Private => TypeEntryAccessibility.Private,
            Accessibility.Protected => TypeEntryAccessibility.Protected,
            Accessibility.Public => TypeEntryAccessibility.Public,
            Accessibility.ProtectedAndInternal => TypeEntryAccessibility.PrivateProtected,
            Accessibility.ProtectedOrInternal => TypeEntryAccessibility.ProtectedInternal,
            _ => TypeEntryAccessibility.NotApplicable
        };
    }

    private IEnumerable<TypeEntry> OrderTypes(IEnumerable<TypeEntry> entries)
        => entries
            .OrderBy(item => item.DisplayName, StringComparer.Ordinal)
            .ToArray();

}