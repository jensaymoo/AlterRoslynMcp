using System.ComponentModel;
using ModelContextProtocol.Server;
using RoslynMcp.Core;
using RoslynMcp.Core.Contracts;
using RoslynMcp.Core.Models;

namespace RoslynMcp.Host.Tools.Inspections;

[McpServerToolType]
public sealed class RunTestsTool(ITestInspectionService testInspectionService)
{
    private readonly ITestInspectionService _testInspectionService = 
        testInspectionService ?? throw new ArgumentNullException(nameof(testInspectionService));

    [McpServerTool(Name = "run_tests", Title = "Run Tests", ReadOnly = true, Idempotent = true)]
    [Description("Default .NET test runner. Use this instead of 'dotnet test' unless you need unsupported CLI behavior.")]
    public Task<RunTestsResult> ExecuteAsync(
        CancellationToken cancellationToken,

        [Description(
            """
            Optional execution target. Omit to run the currently loaded solution. Supports solution-relative or 
            absolute .sln, .slnx, .csproj, or directory paths when the resolved target stays within the loaded solution directory.
            """
        )]
        string target,

        [Description("Optional dotnet test filter expression. Passed through to --filter semantics where practical.")]
        string? filter = null)
    {
        return _testInspectionService.RunTestsAsync(target.ToRunTestsRequest(filter), cancellationToken);
    }
}
