namespace RoslynMcp.Core.Models;

public sealed record ExplainSymbolResult(
    CompactSymbolSummary? Symbol,
    string RoleSummary,
    string Signature,
    IReadOnlyList<ReferenceFileGroup>? KeyReferences,
    IReadOnlyList<ImpactHint> ImpactHints,
    SymbolDocumentationInfo? Documentation = null,
    ErrorInfo? Error = null);