using System.ComponentModel;
using ModelContextProtocol.Server;
using RoslynMcp.Core;
using RoslynMcp.Core.Contracts;
using RoslynMcp.Core.Models;

namespace RoslynMcp.Host.Tools.Inspections;

[McpServerToolType]
public sealed class GetTypeHierarchyTool(INavigationService navigationService)
{
    private readonly INavigationService _navigationService = 
        navigationService ?? throw new ArgumentNullException(nameof(navigationService));

    [McpServerTool(Name = "get_type_hierarchy", Title = "Get Type Hierarchy", ReadOnly = true, Idempotent = true)]
    [Description(
        """
        Use this tool when you need to inspect a type's inheritance relationships: base types, implemented 
        interfaces, and derived types. Use includeTransitive=false for immediate parents and children only, 
        or true to expand the full transitive hierarchy.
        """
    )]
    public Task<GetTypeHierarchyResult> ExecuteAsync(CancellationToken cancellationToken,
        [Description(
            """
            The stable symbol ID of a type, obtained from resolve_symbol, list_types, or list_members. 
            Must resolve to a type (class, interface, enum, struct, or record).
            """
        )]
        string symbolId,

        [Description(
            """
            When true, includes all transitive base types and all derived types. When false, 
            returns only immediate parents and children. Defaults to true.
            """
        )]
        bool includeTransitive = true,

        [Description("Maximum number of derived types to return. Higher values may impact performance. Defaults to 200.")]
        int maxDerived = 200)
    {
        return _navigationService.GetTypeHierarchyAsync(symbolId.ToGetTypeHierarchyRequest(includeTransitive, maxDerived),
            cancellationToken);
    }
}