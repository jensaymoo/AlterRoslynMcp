using RoslynMcp.Core.Models;

namespace RoslynMcp.Core.Contracts;

public interface IWorkspaceBootstrapService: IBaseService
{
    Task<LoadSolutionResult> LoadSolutionAsync(LoadSolutionRequest request, CancellationToken ct);
}