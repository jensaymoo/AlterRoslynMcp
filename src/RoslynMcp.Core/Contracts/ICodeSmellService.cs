using RoslynMcp.Core.Models;

namespace RoslynMcp.Core.Contracts;

public interface ICodeSmellService: IBaseService
{
    Task<FindCodeSmellsResult> FindCodeSmellsAsync(FindCodeSmellsRequest request, CancellationToken ct);
}