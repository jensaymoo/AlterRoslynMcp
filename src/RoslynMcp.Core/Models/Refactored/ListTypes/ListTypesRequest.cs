namespace RoslynMcp.Core.Models;

public sealed record ListTypesRequest(
    string? ProjectPath = null,
    string? ProjectName = null,
    string? ProjectId = null,
    string? NamespacePrefix = null,
    string? Kind = null,
    string? Accessibility = null,
    bool IncludeSummary = false,
    bool IncludeMembers = false,
    int? Limit = null,
    int? Offset = null);