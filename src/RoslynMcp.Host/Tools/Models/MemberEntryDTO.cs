using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record MemberEntryDTO
(
    string DisplayName,
    string Kind,
    string Signature,
    SourceLocationDTO? Location,
    string Accessibility,
    bool IsStatic,
    bool IsInherited,
    string? Summary
);
