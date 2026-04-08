using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using RoslynMcp.Host.Tools;
using RoslynMcp.Host.Tools.Inspections;

namespace RoslynMcp.Host;

internal static class HostExtensions
{
    private static string ServerVersion => Assembly.GetExecutingAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(HostExtensions).Assembly.GetName().Version?.ToString()
        ?? "0.0.0";

    extension(IServiceCollection services)
    {
        internal void AddMcpRuntime()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = false
            };

            var builder = services.AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = nameof(RoslynMcp),
                    WebsiteUrl = "https://github.com/jensaymoo/AlterRoslynMcp",
                    Description = "An MCP server that provides AI agents with Roslyn-based code analysis capabilities",
                    Version = ServerVersion,
                };
            });

            builder.WithStdioServerTransport();
            builder.WithTools<LoadSolutionTool>(serializerOptions);
            builder.WithTools<ListTypesTool>(serializerOptions);
            
            //builder.WithToolsFromAssembly(serializerOptions: serializerOptions);
        }
    }
}