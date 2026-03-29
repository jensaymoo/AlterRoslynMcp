namespace RoslynMcp.Core.Models;

public sealed record ResultContextMetadata(
    string SourceBias,
    string ResultCompleteness,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> DegradedReasons,
    string? RecommendedNextStep = null);