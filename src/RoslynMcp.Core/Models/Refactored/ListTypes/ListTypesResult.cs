namespace RoslynMcp.Core.Models;

public sealed record ListTypesResult(
    IReadOnlyList<TypeListEntry> Types,
    int TotalCount,
    ResultContextMetadata Context,
    ErrorInfo? Error = null);