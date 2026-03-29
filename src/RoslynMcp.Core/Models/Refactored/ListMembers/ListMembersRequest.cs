namespace RoslynMcp.Core.Models;

public sealed record ListMembersRequest(
    string? TypeSymbolId = null,
    string? Path = null,
    int? Line = null,
    int? Column = null,
    string? Kind = null,
    string? Accessibility = null,
    string? Binding = null,
    bool IncludeInherited = false,
    int? Limit = null,
    int? Offset = null);