namespace RoslynMcp.Infrastructure._Refactored;

public sealed class MemberEntry
{
    public required string DisplayName { get; init; }
    public required string Signature { get; init; }
    public required MemberEntryKind Kind { get; init; }
    public required SymbolAccessibility Accessibility { get; init; }
    public required bool IsStatic { get; init; }
    public required bool IsInherited { get; init; }
    public required SourceLocation? Location { get; init; }
    public required string? Summary { get; init; }
}
