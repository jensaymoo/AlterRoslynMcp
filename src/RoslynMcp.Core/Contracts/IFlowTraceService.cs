using RoslynMcp.Core.Models;

namespace RoslynMcp.Core.Contracts;

public interface IFlowTraceService: IBaseService
{
    Task<TraceFlowResult> TraceFlowAsync(TraceFlowRequest request, CancellationToken ct);
}