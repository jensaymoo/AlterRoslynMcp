namespace RoslynMcp.Core.Models;

public sealed record ListMembersResult(
    IReadOnlyList<MemberListEntry> Members,
    int TotalCount,
    bool IncludeInherited,
    ErrorInfo? Error = null);