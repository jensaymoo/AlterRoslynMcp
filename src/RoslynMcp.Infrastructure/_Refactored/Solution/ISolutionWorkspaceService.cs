using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ISolutionWorkspaceService
{
    Task<Solution> LoadSolutionAsync(string slnFilePath, CancellationToken ct);
    Task<Solution> LoadProjectAsync(string csprojFilePath, CancellationToken ct);
    
    Solution GetCurrentSolution();
}