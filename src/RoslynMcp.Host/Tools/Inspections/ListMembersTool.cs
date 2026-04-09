using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using RoslynMcp.Host.Tools.Models;
using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Inspections;

[McpServerToolType]
public sealed class ListMembersTool(IMembersEnumerationService membersEnumerationService)
{
    [McpServerTool(Name = "list_members", Title = "List Members", ReadOnly = true, Idempotent = true)]
    [Description(
        """
        Use this tool when you need to inspect the members declared by a specific type. It returns methods, 
        properties, fields, events, and constructors, and supports filtering by kind, accessibility, and inheritance.
        """
    )]
    public async Task<IEnumerable<MemberEntryDTO>> ExecuteAsync(CancellationToken ct,
        [Description("Full name of the type (e.g., RoslynMcp.Infrastructure.Service).")]
        string fullTypeName,

        [Description("Filter by member kind: method, property, field, event, or constructor.")]
        MemberEntryKind? kind = null,

        [Description("Filter by accessibility: public, internal, protected, private, protected_internal, or private_protected.")]
        SymbolAccessibility? accessibility = null,

       

        [Description("When true, includes members from base classes. Defaults to false.")]
        bool includeInherited = false,

        [Description("When true, includes XML documentation summaries for returned members when available.")]
        bool includeSummary = false)
    {
        try
        {
            var members = await membersEnumerationService.EnumerateMembersAsync(
                fullTypeName,
                kind,
                accessibility,
                includeInherited,
                includeSummary,
                ct);

            return members.Select(m => new MemberEntryDTO(
                m.DisplayName,
                m.Kind.ToString().ToLowerInvariant(),
                m.Signature,
                m.Location != null ? new SourceLocationDTO(m.Location.FilePath, m.Location.Column, m.Location.Line) : null,
                m.Accessibility.ToString().ToLowerInvariant(),
                m.IsStatic,
                m.IsInherited,
                m.Summary
            ));
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
