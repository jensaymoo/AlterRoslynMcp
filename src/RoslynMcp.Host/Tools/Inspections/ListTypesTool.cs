using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Inspections;


public sealed record ListTypesResult(
    ProjectSummary Project,
    IEnumerable<TypeEntry> Types);

[McpServerToolType]
public sealed class ListTypesTool(ITypeEnumerationService typeEnumerationService)
{
    [McpServerTool(Name = "list_types", Title = "List Types", ReadOnly = true, Idempotent = true)]
    [Description("Use this tool when you need to list types declared in a specific loaded project. It is useful for " +
                 "project-scoped discovery, for finding type symbols before follow-up calls such as list_members or " +
                 "resolve_symbol, and for optionally enriching only the returned type entries with XML summaries or " +
                 "lightweight declared-member previews. For automation, prefer projectPath as the stable selector; " +
                 "projectId is snapshot-local to the active workspace snapshot. Results prefer handwritten declarations " +
                 "by default and report source bias, completeness, and degraded discovery hints.")]
    public async Task<IEnumerable<ListTypesResult>> ExecuteAsync(CancellationToken ct,
        /*[Description("Exact path to a project file (.csproj). Specify only one of projectPath, projectName, or projectId." )]
        string? projectPath = null,*/

        [Description("Name of a project. Specify only one of projectPath, projectName, or projectId." )]
        string projectName,

        /*[Description(
            """
            Project identifier from the current loaded workspace snapshot. projectId values are snapshot-local 
            and can change after reload, so prefer projectPath for durable automation. Specify only one of 
            projectPath, projectName, or projectId.
            """
        )]
        string? projectId = null,*/

        [Description("Filter to only types in namespaces starting with this prefix." )]
        string? namespacePrefix = null,

        [Description("Filter by type kind: class, record, interface, enum, or struct." )]
        TypeEntryKind? kind = null,

        [Description("Filter by accessibility: public, internal, protected, private, protected_internal, or private_protected.")]
        TypeEntryAccessibility? accessibility = null,

        [Description(
            """
            When omitted or true, includes XML documentation summaries for returned type entries when available. 
            Pass false to omit summaries. Defaults to false.
            """
        )]
        bool includeSummary = false,

        [Description(
            """
            When true, includes a lightweight preview of declared members for each returned type entry. 
            This is not full member metadata: each member is returned as a single normalized accessibility-plus-signature 
            string, and only members declared on that type are included. Enrichment is applied only to the type entries 
            returned on the current page. Use list_members as the detailed follow-up tool. When omitted or false, 
            members are omitted. Defaults to false.
            """
        )]
        bool includeMembers = false)
    {
        try
        {
            var allTypes = await typeEnumerationService.EnumerateTypesBySolutionAsync(includeSummary, ct);
            
            var filteredByProject = allTypes
                .Where(x => x.ProjectName.Equals(projectName.Trim(), StringComparison.OrdinalIgnoreCase));

            var filtered = filteredByProject
                .Where(x => string.IsNullOrEmpty(namespacePrefix) || 
                            x.Namespace?.StartsWith(namespacePrefix) == true)
                .Where(x => kind == null || x.Kind.Equals(kind))
                .Where(x => accessibility == null || x.Accessibility.Equals(accessibility));

            var groupedByProject = filtered
                .GroupBy(x => x.ProjectName)
                .Select(g => new ListTypesResult(
                    new ProjectSummary(g.Key, g.First().ProjectPath),
                    g.AsEnumerable()));

            return groupedByProject;
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
