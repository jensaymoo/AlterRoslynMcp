namespace RoslynMcp.Core.Models;

public sealed record UnderstandProjectsResult(
    string Profile,
    IReadOnlyList<ProjectLandscapeSummary> Projects,
    IReadOnlyList<HotspotSummary> Hotspots,
    ErrorInfo? Error = null);