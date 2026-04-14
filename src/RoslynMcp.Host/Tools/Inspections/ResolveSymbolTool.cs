using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using RoslynMcp.Host.Tools.Models;
using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Inspections;

[McpServerToolType]
public sealed class ResolveSymbolTool(IResolveSymbolService resolveSymbolService)
{
    [McpServerTool(Name = "resolve_symbol", Title = "Resolve Symbol", ReadOnly = true, Idempotent = true)]
    [Description(
        """
        Use this tool when you have a symbolId and need to resolve it to its declaration location
        and metadata. Returns the symbol's display name, kind, source locations and owning project.
        """
    )]
    public async Task<IEnumerable<ResolvedSymbolDTO>> ExecuteAsync(CancellationToken ct,
        [Description("Symbol ID to resolve (e.g., T:MyNamespace.MyType or M:MyNamespace.MyType.MyMethod).")]
        string symbolId)
    {
        try
        {
            var results = await resolveSymbolService.ResolveAsync(symbolId, ct);

            return results.Select(r => new ResolvedSymbolDTO(
                SymbolId: r.SymbolId,
                DisplayName: r.DisplayName,
                Kind: r.Kind,
                Location: r.Location
                    .Select(loc => new SourceLocationDTO(loc.FilePath, loc.Line, loc.Column))
                    .DistinctBy(loc => new { loc.FilePath, loc.Line, loc.Column }),
                ProjectName: r.ProjectName));
        }
        catch (SolutionNotLoadedException)
        {
            throw new McpException("""
                                   Solution not loaded. Before performing any operations, you must first load the
                                   project using the `load_solution` tool. Ensure the solution is successfully loaded
                                   and available in the current context, then proceed with further actions.
                                   """);
        }
        catch (Exception e)
        {
            throw new McpException(e.Message, e);
        }
    }
}
