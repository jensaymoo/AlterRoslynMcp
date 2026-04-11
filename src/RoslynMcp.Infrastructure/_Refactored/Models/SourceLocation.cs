namespace RoslynMcp.Infrastructure._Refactored;

public sealed class SourceLocation
{
    public required string FilePath { get; init; }
    public required int Line { get; init; }
    public required int Column { get; init; }
}