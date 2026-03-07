using Xunit;
using Xunit.Abstractions;

namespace RoslynMcp.Features.Tests;

[Collection(SharedSandboxFeatureTestsCollection.Name)]
public abstract class SandboxedToolTests<TTool> where TTool : notnull
{
    private readonly ITestOutputHelper _output;

    protected SandboxedToolTests(SharedSandboxFeatureTestsFixture fixture, ITestOutputHelper output)
    {
        Fixture = fixture;
        _output = output;
        Sut = fixture.GetRequiredService<TTool>();
    }

    protected SharedSandboxFeatureTestsFixture Fixture { get; }

    protected TTool Sut { get; }

    protected string TestSolutionDirectory => Fixture.TestSolutionDirectory;

    protected string CodeSmellsPath => Fixture.GetFilePath("ProjectImpl", "CodeSmells");
    protected string AppOrchestratorPath => Fixture.GetFilePath("ProjectApp", "AppOrchestrator");
    protected string HierarchyPath => Fixture.GetFilePath("ProjectCore", "Hierarchy");
    protected string ContractsPath => Fixture.GetFilePath("ProjectCore", "Contracts");

    protected string GetFilePath(string project, string file) => Fixture.GetFilePath(project, file);

    protected void Trace(string message) => _output.WriteLine(typeof(TTool) + ": " + message);
}
