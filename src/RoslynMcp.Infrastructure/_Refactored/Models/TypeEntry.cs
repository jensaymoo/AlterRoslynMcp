using Microsoft.CodeAnalysis;
using System.Linq;

namespace RoslynMcp.Infrastructure._Refactored;

public sealed class TypeEntry
{
    public string SymbolName { get; init; }
    public TypeEntryKind Kind { get; init; }
    public SymbolAccessibility Accessibility { get; init; }
    
    public string? Namespace { get; init; }
    public IEnumerable<TypeEntry>? BaseTypes { get; init; }
    public IEnumerable<SourceLocation> Location { get; init; }
    public string? Summary { get; init; }
    
    public string ProjectName { get; init; }
    public string? ProjectPath { get; init; }

    public TypeEntry(INamedTypeSymbol symbol, Project project)
    {
        SymbolName = symbol.GetSymbolName();
        Kind = symbol.GetTypeEntryKind();
        Accessibility = symbol.GetSymbolAccessibility();
        Namespace = symbol.ContainingNamespace.GetSymbolNameOrNull();
        Location = symbol.Locations.AsSourceLocations();
        Summary = symbol.GetDocumentationCommentXml();

        ProjectName = project.Name;
        ProjectPath = project.FilePath;

        BaseTypes = GetDirectBaseTypes(symbol, project);
    }

    private static IEnumerable<TypeEntry>? GetDirectBaseTypes(INamedTypeSymbol type, Project project)
    {
        if (type.TypeKind == TypeKind.Enum || !project.TryGetCompilation(out var compilation))
            return null;

        var entries = GetDirectBaseSymbols(type)
            .Select(t => CreateTypeEntryOrNull(t, project, compilation))
            .OfType<TypeEntry>()
            .ToList();

        return entries.Count > 0 ? entries : null;
    }

    private static IEnumerable<INamedTypeSymbol> GetDirectBaseSymbols(INamedTypeSymbol type)
    {
        if (type.BaseType is { IsImplicitlyDeclared: false } baseType)
            yield return baseType;

        foreach (var iface in type.Interfaces.Where(i => !i.IsImplicitlyDeclared))
            yield return iface;
    }

    private static TypeEntry? CreateTypeEntryOrNull(INamedTypeSymbol symbol, Project project, Compilation compilation)
    {
        var syntaxRef = symbol.DeclaringSyntaxReferences.FirstOrDefault();
        var isFromProject = syntaxRef is not null && compilation.SyntaxTrees.Contains(syntaxRef.SyntaxTree);

        return isFromProject ? new TypeEntry(symbol, project) : null;
    }
}
