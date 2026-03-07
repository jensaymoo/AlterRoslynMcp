using Xunit.Abstractions;

namespace RoslynMcp.Features.Tests;

public abstract class IsolatedToolTests<TTool> where TTool : notnull
{
    private readonly ITestOutputHelper _output;

    protected IsolatedToolTests(ITestOutputHelper output)
    {
        _output = output;
    }

    protected Task<IsolatedSandboxFeatureTestContext> CreateContextAsync(CancellationToken cancellationToken = default)
        => IsolatedSandboxFeatureTestContext.CreateAsync(cancellationToken);

    protected static TTool GetSut(IsolatedSandboxFeatureTestContext context)
        => context.GetRequiredService<TTool>();

    protected void Trace(string message) => _output.WriteLine(typeof(TTool) + ": " + message);
}
