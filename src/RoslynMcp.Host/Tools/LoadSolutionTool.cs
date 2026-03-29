using System.ComponentModel;
using ModelContextProtocol.Server;
using RoslynMcp.Core;
using RoslynMcp.Core.Contracts;
using RoslynMcp.Core.Models;

namespace RoslynMcp.Host.Tools;

[McpServerToolType]
public sealed class LoadSolutionTool(IWorkspaceBootstrapService workspaceBootstrapService)
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
    public Task<LoadSolutionResult> ExecuteAsync(CancellationToken cancellationToken,
        [Description(
            """
            Absolute path to a `.sln` file, or to a directory used as the recursive discovery 
            root for `.sln`/`.slnx` files.
            """
        )]
        string solutionHintPath)
    {
        return workspaceBootstrapService.LoadSolutionAsync(solutionHintPath.ToLoadSolutionRequest(),
            cancellationToken);
    }
}