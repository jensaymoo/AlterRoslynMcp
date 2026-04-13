using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record MemberEntryDTO
(
    string SymbolId,
    string SymbolName,
    MemberEntryKind Kind,
    string Signature,
    IEnumerable<SourceLocationDTO>? Location,
    SymbolAccessibility Accessibility,
    bool IsStatic,
    bool IsInherited,
    string? Summary,
    bool IsVirtual,
    bool IsOverride,
    bool IsAbstract,
    bool IsSealed,
    bool IsExtern
);
