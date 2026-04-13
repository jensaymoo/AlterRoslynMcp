using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record TypeEntryDTO
(
    string SymbolId,
    string SymbolName,
    string? Namespace,
    IEnumerable<SourceLocationDTO> Location,
    SymbolAccessibility Accessibility,
    TypeEntryKind Kind,
    string? Summary,
    IEnumerable<TypeEntryDTO>? BaseTypes = null
);
