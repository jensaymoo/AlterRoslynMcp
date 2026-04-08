using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoslynMcp.Infrastructure;
using RoslynMcp.Infrastructure._Refactored;
using HostService = Microsoft.Extensions.Hosting.Host;

namespace RoslynMcp.Host;

public static class McpServerHost
{
    public static async Task RunAsync(string[] args, CancellationToken ct = default)
    {
        var builder = HostService.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(o 
            => o.LogToStandardErrorThreshold = LogLevel.Trace);

        builder.Services.AddSingleton<ISolutionWorkspaceService, SolutionWorkspaceService>();
        
        builder.Services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        builder.Services
            .AddInfrastructure()
            .AddMcpRuntime();

        var host = builder.Build();
        
        await host.RunAsync(ct);
    }
}
