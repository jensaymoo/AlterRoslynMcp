namespace RoslynMcp.Infrastructure._Refactored;

public class Diagnostic
{
    public required string Code { get; init; }
    public required Severity Severity { get; init; }
    public required string Message { get; init; }
    public required SourceLocation? Location { get; init; }
}