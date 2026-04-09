using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record TypeEntryDTO
(
    string SymbolName,
    IEnumerable<SourceLocationDTO> Location,
    TypeEntryAccessibility Accessibility,
    TypeEntryKind Kind,
    string? Summary
);