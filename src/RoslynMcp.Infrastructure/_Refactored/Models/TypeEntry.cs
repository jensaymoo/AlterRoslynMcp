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
    }
}
