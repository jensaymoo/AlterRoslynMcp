namespace RoslynMcp.Infrastructure._Refactored;

public interface IMembersEnumerationService : IScopedService
{
    Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string fullTypeName,
        MemberEntryKind? kind,
        SymbolAccessibility? accessibility,
        string? binding,
        bool includeInherited,
        bool includeSummary,
        CancellationToken ct);
}
