namespace RoslynMcp.Infrastructure._Refactored;

public interface IMembersEnumerationService : IScopedService
{
    Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolId, MemberEntryKind? kind, SymbolAccessibility? accessibility, bool includeInherited,
        CancellationToken ct = default);

    Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolId, string projectName, MemberEntryKind? kind, SymbolAccessibility? accessibility,
        bool includeInherited, CancellationToken ct = default);
}
