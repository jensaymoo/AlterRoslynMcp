using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using RoslynMcp.Infrastructure._Refactored.Exceptions;

namespace RoslynMcp.Infrastructure._Refactored;

public class SolutionWorkspaceService(ILogger<SolutionWorkspaceService> logger): ISolutionWorkspaceService
{
    private Solution? _solution;
    private MSBuildWorkspace? _workspace;
    
    static SolutionWorkspaceService()
    {
        MSBuildLocator.RegisterDefaults();
    }
    
    public Solution GetCurrentSolution()
    {
        if (_solution is not null)
            return _solution;
        
        throw new SolutionNotLoadedException("Solution not loaded");
    }
    
    public async Task<Solution> LoadSolutionAsync(string solutionFilePath, CancellationToken ct = default)
    {
        if (_workspace is not null)
        {
            _workspace.CloseSolution();
            _workspace.Dispose();
        }
        
        try
        {
            _workspace = MSBuildWorkspace.Create();
            _solution = await _workspace
                .OpenSolutionAsync(solutionFilePath, cancellationToken: ct);
            
            return _solution;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,  "Filed to load solution");
            throw new LoadSolutionException(ex.Message, ex);
        }
    }

    public async Task<Solution> LoadProjectAsync(string csprojFilePath, CancellationToken ct = default)
    {
        if (_workspace is not null)
        {
            _workspace.CloseSolution();
            _workspace.Dispose();
        }
        
        try
        {
            _workspace = MSBuildWorkspace.Create();
            
            var project = await _workspace.OpenProjectAsync(csprojFilePath, cancellationToken: ct);
            _solution = project.Solution;
            
            return _solution;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,  "Filed to load project");
            throw new LoadSolutionException(ex.Message, ex);
        }
    }
}