using RoslynMcp.Core.Models;

namespace RoslynMcp.Core.Contracts;

public interface ICodeUnderstandingService: IBaseService
{
    Task<UnderstandProjectsResult> UnderstandProjectsAsync(UnderstandProjectsRequest request, CancellationToken ct);
    Task<ExplainSymbolResult> ExplainSymbolAsync(ExplainSymbolRequest request, CancellationToken ct);
    Task<ListTypesResult> ListTypesAsync(ListTypesRequest request, CancellationToken ct);
    Task<ListMembersResult> ListMembersAsync(ListMembersRequest request, CancellationToken ct);
    Task<ResolveSymbolResult> ResolveSymbolAsync(ResolveSymbolRequest request, CancellationToken ct);
    Task<ResolveSymbolsBatchResult> ResolveSymbolsBatchAsync(ResolveSymbolsBatchRequest request, CancellationToken ct);
}