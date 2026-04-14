using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record ResolvedSymbolDTO(
    string SymbolId,
    string DisplayName,
    SymbolEntryKind Kind,
    IEnumerable<SourceLocationDTO> Location,
    string ProjectName);
