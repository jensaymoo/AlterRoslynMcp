namespace RoslynMcp.Infrastructure._Refactored;

public sealed class TypeEntry
{
    public required string SymbolName { get; init; }
    public required string Namespace { get; init; }
    
    public required IEnumerable<SourceLocation> Location { get; init; }
    
    public required SymbolAccessibility Accessibility { get; init; }
    public required TypeEntryKind Kind { get; init; }
    
    public required string? Summary { get; init; }
    
    public required string ProjectName { get; init; }
    public required string? ProjectPath { get; init; }
}
