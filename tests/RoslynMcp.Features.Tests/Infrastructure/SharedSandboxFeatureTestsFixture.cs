using Xunit;

namespace RoslynMcp.Features.Tests;

[CollectionDefinition(Name)]
public sealed class SharedSandboxFeatureTestsCollection : ICollectionFixture<SharedSandboxFeatureTestsFixture>
{
    public const string Name = "FeatureTests";
}

public sealed class SharedSandboxFeatureTestsFixture : FeatureTestFixtureBase, IAsyncLifetime
{
    public Task InitializeAsync()
        => InitializeSandboxAsync(TestSolutionSandbox.Create(CanonicalTestSolutionDirectory));

    Task IAsyncLifetime.DisposeAsync()
        => DisposeAsync().AsTask();
}
