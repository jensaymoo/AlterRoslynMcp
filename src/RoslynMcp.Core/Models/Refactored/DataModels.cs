namespace RoslynMcp.Core.Models;

public sealed record ProjectSummary(string Name, string? Path);

public sealed record DiagnosticsSummary(int ErrorCount, int WarningCount, int InfoCount, int TotalCount);

public sealed record ProjectLandscapeSummary(
    string Name,
    string? ProjectPath,
    IReadOnlyList<string> OutgoingDependencyProjectPaths,
    IReadOnlyList<string> IncomingDependencyProjectPaths,
    IReadOnlyList<string> Types);

public sealed record HotspotSummary(
    string Display,
    string Reason,
    int Score,
    string SymbolId,
    SourceLocation? Location,
    int Complexity,
    int LineCount);

public sealed record TypeListEntry(
    string DisplayName,
    string SymbolId,
    SourceLocation? Location,
    string Kind,
    int? Arity,
    string? Summary = null,
    IReadOnlyList<string>? Members = null);

public sealed record MemberListEntry(
    string DisplayName,
    string SymbolId,
    string Kind,
    string Signature,
    SourceLocation? Location,
    string Accessibility,
    bool IsStatic);

public sealed record ResolvedSymbolSummary(
    string SymbolId,
    string DisplayName,
    string Kind,
    SourceLocation? Location,
    string? QualifiedDisplayName = null);

public sealed record ResolveSymbolCandidate(
    string SymbolId,
    string DisplayName,
    string Kind,
    SourceLocation? Location,
    string ProjectName,
    string? QualifiedDisplayName = null);

public sealed record ResolveSymbolsBatchItemResult(
    int Index,
    string? Label,
    ResolvedSymbolSummary? Symbol,
    bool IsAmbiguous,
    IReadOnlyList<ResolveSymbolCandidate> Candidates,
    ErrorInfo? Error = null);

public sealed record ImpactHint(string Zone, string Reason, int ReferenceCount);

public sealed record SymbolDocumentationParameter(
    string Name,
    string Description);

public sealed record SymbolDocumentationInfo(
    string? Summary = null,
    string? Returns = null,
    IReadOnlyList<SymbolDocumentationParameter>? Parameters = null);

public sealed record FlowTransition(
    string FromProject,
    string ToProject,
    int Count,
    IReadOnlyList<string>? UncertaintyCategories = null);

public sealed record TraceRootSummary(
    string Name,
    string Kind,
    string? Owner,
    SourceLocation? Location);

public sealed record TraceSymbolEntry(
    string Display,
    SourceLocation? Location);

public sealed record TraceFlowEdge(
    string From,
    string To,
    SourceLocation Site,
    string Kind,
    IReadOnlyList<string>? UncertaintyCategories = null,
    IReadOnlyList<string>? RelatedSymbolIds = null);

public sealed record FlowUncertainty(
    string Category,
    string Message,
    SourceLocation? Location = null,
    SymbolReference? RelatedSymbol = null);

public sealed record CodeSmellsSummary(
    int TotalFindings,
    int TotalOccurrences);

public sealed record CodeSmellOccurrenceFile(
    string FilePath,
    IReadOnlyList<ReferencePosition> Locations);

public sealed record CodeSmellFindingEntry(
    string FindingKey,
    string Title,
    string RiskLevel,
    string Category,
    string ReviewKind,
    int OccurrenceCount,
    IReadOnlyList<CodeSmellOccurrenceFile> OccurrenceFiles);

public sealed record DiagnosticItem(string Code, string Severity, string Message, SourceLocation Location);

public sealed record MetricItem(string SymbolId, int? CyclomaticComplexity, int? LineCount);

public sealed record SymbolDescriptor(
    string SymbolId,
    string Name,
    string Kind,
    string? ContainingType,
    string? ContainingNamespace,
    SourceLocation DeclarationLocation);

public sealed record UsageSymbolSummary(
    string SymbolId,
    string Display,
    string Kind,
    SourceLocation? Location);

public sealed record ReferencePosition(
    int Line,
    int Column);

public sealed record ReferenceFileGroup(
    string FilePath,
    IReadOnlyList<ReferencePosition> References);

public sealed record CompactSymbolSummary(
    string SymbolId,
    string Display,
    string Kind,
    SourceLocation? Location,
    string? Owner = null);

public sealed record SymbolMemberOutline(string Name, string Kind, string Signature, string Accessibility, bool IsStatic);

public sealed record CallEdge(
    string FromSymbolId,
    string ToSymbolId,
    SourceLocation Location,
    SymbolReference? FromReference = null,
    SymbolReference? ToReference = null,
    string EvidenceKind = FlowEvidenceKinds.DirectStatic,
    IReadOnlyList<FlowUncertainty>? Uncertainties = null,
    IReadOnlyList<SymbolReference>? PossibleTargets = null);

public sealed record MethodInsertionSpec(
    string Name,
    string ReturnType,
    string Accessibility,
    IReadOnlyList<string> Modifiers,
    IReadOnlyList<MethodParameterSpec> Parameters,
    string Body,
    string? Placement = null);

public sealed record MethodParameterSpec(
    string Name,
    string Type);

public sealed record AddedMethodInfo(
    string SymbolId,
    string Signature);

public sealed record DeletedMethodInfo(
    string SymbolId,
    string Signature);

public sealed record ReplacedMethodInfo(
    string OriginalSymbolId,
    string OriginalSignature,
    string NewSymbolId,
    string NewSignature);

public sealed record ReplacedMethodBodyInfo(
    string MethodSymbolId,
    string Signature);

public sealed record MutationDiagnosticInfo(
    string Id,
    string Severity,
    string Message,
    string FilePath,
    int Line,
    int Column,
    string? Origin = null);

public sealed record DiagnosticsDeltaInfo(
    IReadOnlyList<MutationDiagnosticInfo> NewErrors,
    IReadOnlyList<MutationDiagnosticInfo> NewWarnings);

public sealed record AffectedFileLocations(
    string FilePath,
    IReadOnlyList<ReferencePosition> Locations);

public sealed record CodeFixDescriptor(
    string FixId,
    string Title,
    string DiagnosticId,
    string Category,
    SourceLocation Location,
    string FilePath);

public sealed record ChangedFilePreview(string FilePath, int EditCount);

public sealed record PolicyDecisionInfo(string Decision, string ReasonCode, string ReasonMessage);

public sealed record RefactoringActionDescriptor(
    string ActionId,
    string Title,
    string Category,
    string Origin,
    string RiskLevel,
    PolicyDecisionInfo PolicyDecision,
    SourceLocation Location,
    string? DiagnosticId = null,
    string? RefactoringId = null);

public sealed record SourceLocation(string FilePath, int Line, int Column);

public sealed record SymbolReference(
    string SymbolId,
    string Handle,
    string QualifiedDisplayName,
    SourceLocation? DeclarationLocation = null);

public sealed record ErrorInfo(string Code, string Message, IReadOnlyDictionary<string, string>? Details = null);

public sealed record TestFailureGroup(
    string? File,
    int Count,
    IReadOnlyList<GroupedTestFailure> Failures);

public sealed record GroupedTestFailure(
    string? TestName,
    string? Message,
    int? Line);

public sealed record TestRunCounts(
    int Total,
    int Executed,
    int Passed,
    int Failed,
    int Skipped,
    int NotExecuted);

public sealed record BuildDiagnostic(
    string? Id,
    string? Message,
    string? File,
    int? Line,
    int? Column,
    string? Severity);
