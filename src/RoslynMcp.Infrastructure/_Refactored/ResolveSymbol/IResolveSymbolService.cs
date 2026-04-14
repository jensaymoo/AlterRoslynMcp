namespace RoslynMcp.Infrastructure._Refactored;

public interface IResolveSymbolService : IScopedService
{
    Task<IEnumerable<ResolvedSymbolEntry>> ResolveAsync(
        string symbolId,
        CancellationToken ct);
}
