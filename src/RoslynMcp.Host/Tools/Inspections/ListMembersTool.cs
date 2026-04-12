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
        string symbolName,
        
        [Description("Name of a project. When omitted or empty, returns types from all projects.")]
        string? projectName = null,
        
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
            var members = string.IsNullOrWhiteSpace(projectName)
                ? await membersEnumerationService.EnumerateMembersAsync(symbolName, kind, accessibility, includeInherited, ct)
                : await membersEnumerationService.EnumerateMembersAsync(symbolName, projectName, kind, accessibility, includeInherited, ct);

            return members.Select(m => new MemberEntryDTO(
                SymbolName: m.SymbolName,
                Kind: m.Kind,
                Signature: m.Signature,
                Location: m.Location?
                    .Select(x => new SourceLocationDTO(x.FilePath, x.Column, x.Line))
                    .DistinctBy(loc => new { loc.FilePath, loc.Line, loc.Column }),
                Accessibility: m.Accessibility,
                Summary: includeSummary ? m.Summary : null,
                IsStatic: m.IsStatic,
                IsInherited: m.IsInherited,
                IsVirtual: m.IsVirtual,
                IsOverride: m.IsOverride,
                IsAbstract: m.IsAbstract,
                IsSealed: m.IsSealed,
                IsExtern: m.IsExtern
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
