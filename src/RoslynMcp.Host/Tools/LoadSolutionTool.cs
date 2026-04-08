using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools;

public sealed record LoadSolutionResult(string SolutionPath,
    IEnumerable<ProjectSummary> Projects,
    IEnumerable<Diagnostic> BaselineDiagnostics);

public sealed record ProjectSummary(string Name, string? ProjectPath);

[McpServerToolType]
public sealed class LoadSolutionTool(ISolutionWorkspaceService solutionWorkspaceService, IAnalyzeSolutionService analyzeSolutionService)
{
    [McpServerTool(Name = "load_solution", Title = "Load Solution", ReadOnly = false, Idempotent = false)]
    [Description(
        """
        Use this tool when you need to start working with a .NET solution and no solution has been loaded yet. 
        This must be the first tool called in a session before any code analysis or navigation tools can be 
        used. The result now includes a readiness state so fresh or detached worktrees can be reported as 
        degraded_missing_artifacts or degraded_restore_recommended instead of leaving users to infer that from 
        diagnostics alone.
        """
    )]
    public async Task<LoadSolutionResult> ExecuteAsync(CancellationToken ct,
        [Description("Absolute path to a `.sln / .snlx` file" )] string slnFilePath)
    {
        try
        {
            var solution = await solutionWorkspaceService.LoadSolutionAsync(slnFilePath, ct);
            var analyzeResult = await analyzeSolutionService.AnalyzeSolutionAsync(solution, ct);
            
            var projects = solution.Projects
                .OrderBy(static p => p.Name, StringComparer.Ordinal)
                .Select(static p => new ProjectSummary(p.Name, p.FilePath));
            
            return new LoadSolutionResult(solution.FilePath!, projects, analyzeResult);
        }
        catch (Exception e)
        {
            throw new McpException(e.Message, e);
        }
    }
}

