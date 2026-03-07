using Is.Assertions;
using RoslynMcp.Features.Tools;
using Xunit;
using Xunit.Abstractions;

namespace RoslynMcp.Features.Tests.ToolTests;

public sealed class LoadSolutionToolTests(SharedSandboxFeatureTestsFixture fixture, ITestOutputHelper output)
    : SandboxedToolTests<LoadSolutionTool>(fixture, output)
{
    [Fact]
    public void LoadSolutionAsync_WithAbsoluteSolutionPath_LoadsExpectedProjects()
    {
        var result = Fixture.LoadedSolution;

        result.SelectedSolutionPath.Is(Fixture.SolutionPath);
        string.Equals(Fixture.SolutionPath, Fixture.CanonicalSolutionPath, StringComparison.OrdinalIgnoreCase).IsFalse();
        result.Error.ShouldBeNone();

        var projectNames = result.Projects.Select(static project => project.Name).ToArray();
        
        projectNames.IsContaining("ProjectApp");
        projectNames.IsContaining("ProjectCore");
        projectNames.IsContaining("ProjectImpl");
    }
}
