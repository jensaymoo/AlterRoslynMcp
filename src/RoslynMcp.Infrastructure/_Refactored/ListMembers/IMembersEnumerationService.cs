namespace RoslynMcp.Infrastructure._Refactored;

public interface IMembersEnumerationService : IScopedService
{
    Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolName, MemberEntryKind? kind, SymbolAccessibility? accessibility, bool includeInherited, CancellationToken ct = default);
}
