namespace RoslynMcp.Features.Tests;

public sealed class IsolatedSandboxFeatureTestContext : FeatureTestFixtureBase
{
    private IsolatedSandboxFeatureTestContext()
    {
    }

    public static async Task<IsolatedSandboxFeatureTestContext> CreateAsync(CancellationToken cancellationToken = default)
    {
        var context = new IsolatedSandboxFeatureTestContext();
        await context.InitializeSandboxAsync(TestSolutionSandbox.Create(context.CanonicalTestSolutionDirectory), cancellationToken)
            .ConfigureAwait(false);
        return context;
    }
}
