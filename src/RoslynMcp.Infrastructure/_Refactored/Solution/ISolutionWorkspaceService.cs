using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ISolutionWorkspaceService
{
    Task<Solution> LoadSolutionAsync(string slnFilePath, CancellationToken ct = default);
    Task<Solution> LoadProjectAsync(string csprojFilePath, CancellationToken ct = default);
    
    Solution GetCurrentSolution();
}