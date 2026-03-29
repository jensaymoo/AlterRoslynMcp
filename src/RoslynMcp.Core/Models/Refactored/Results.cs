namespace RoslynMcp.Core.Models;

// Per result classes moved to subdirectories
// public sealed record WorkspaceReadiness(
//     string State,
//     IReadOnlyList<string> DegradedReasons,
//     string? RecommendedNextStep = null);

// public sealed record LoadSolutionResult(
//     string? SelectedSolutionPath,
//     string WorkspaceId,
//     string WorkspaceSnapshotId,
//     IReadOnlyList<ProjectSummary> Projects,
//     DiagnosticsSummary BaselineDiagnostics,
//     WorkspaceReadiness Readiness,
//     ErrorInfo? Error = null);

// public sealed record ListTypesResult(
//     IReadOnlyList<TypeListEntry> Types,
//     int TotalCount,
//     ResultContextMetadata Context,
//     ErrorInfo? Error = null);

// public sealed record ListMembersResult(
//     IReadOnlyList<MemberListEntry> Members,
//     int TotalCount,
//     bool IncludeInherited,
//     ErrorInfo? Error = null);

// public sealed record ResolveSymbolResult(
//     ResolvedSymbolSummary? Symbol,
//     bool IsAmbiguous,
//     IReadOnlyList<ResolveSymbolCandidate> Candidates,
//     ErrorInfo? Error = null);

// public sealed record ResolveSymbolsBatchResult(
//     IReadOnlyList<ResolveSymbolsBatchItemResult> Results,
//     int TotalCount,
//     int ResolvedCount,
//     int AmbiguousCount,
//     int ErrorCount,
//     ErrorInfo? Error = null);

// public sealed record UnderstandProjectsResult(
//     string Profile,
//     IReadOnlyList<ProjectLandscapeSummary> Projects,
//     IReadOnlyList<HotspotSummary> Hotspots,
//     ErrorInfo? Error = null);

// public sealed record ExplainSymbolResult(
//     CompactSymbolSummary? Symbol,
//     string RoleSummary,
//     string Signature,
//     IReadOnlyList<ReferenceFileGroup>? KeyReferences,
//     IReadOnlyList<ImpactHint> ImpactHints,
//     SymbolDocumentationInfo? Documentation = null,
//     ErrorInfo? Error = null);

// public sealed record TraceFlowResult(
//     string? RootSymbolId,
//     TraceRootSummary? Root,
//     string Direction,
//     int Depth,
//     IReadOnlyDictionary<string, TraceSymbolEntry>? Symbols,
//     IReadOnlyList<TraceFlowEdge> Edges,
//     IReadOnlyList<TraceFlowEdge>? PossibleTargetEdges = null,
//     IReadOnlyList<FlowTransition>? Transitions = null,
//     IReadOnlyList<string>? RootUncertaintyCategories = null,
//     ErrorInfo? Error = null);

// public sealed record FindCodeSmellsResult(
//     CodeSmellsSummary Summary,
//     IReadOnlyList<CodeSmellFindingEntry> Findings,
//     IReadOnlyList<string>? Warnings,
//     ResultContextMetadata Context,
//     ErrorInfo? Error = null);

public sealed record AnalyzeSolutionResult(IReadOnlyList<DiagnosticItem> Diagnostics, ErrorInfo? Error = null);

public sealed record AnalyzeScopeResult(
    string Scope,
    string? Path,
    IReadOnlyList<DiagnosticItem> Diagnostics,
    IReadOnlyList<MetricItem> Metrics,
    ErrorInfo? Error = null);

public sealed record GetCodeMetricsResult(IReadOnlyList<MetricItem> Metrics, ErrorInfo? Error = null);

public sealed record FindSymbolResult(SymbolDescriptor? Symbol, ErrorInfo? Error = null);

public sealed record GetSymbolAtPositionResult(SymbolDescriptor? Symbol, ErrorInfo? Error = null);

public sealed record SearchSymbolsResult(IReadOnlyList<SymbolDescriptor> Symbols, int TotalCount, ErrorInfo? Error = null);

public sealed record SearchSymbolsScopedResult(IReadOnlyList<SymbolDescriptor> Symbols, int TotalCount, ErrorInfo? Error = null);

public sealed record GetSignatureResult(SymbolDescriptor? Symbol, string Signature, ErrorInfo? Error = null);

public sealed record FindReferencesResult(SymbolDescriptor? Symbol, IReadOnlyList<SourceLocation> References, ErrorInfo? Error = null);

// public sealed record FindReferencesScopedResult(UsageSymbolSummary? Symbol,
//     IReadOnlyList<ReferenceFileGroup> ReferenceFiles,
//     int TotalCount,
//     ErrorInfo? Error = null);

// public sealed record FindImplementationsResult(CompactSymbolSummary? Symbol, IReadOnlyList<CompactSymbolSummary> Implementations, ErrorInfo? Error = null);

// public sealed record GetTypeHierarchyResult(CompactSymbolSummary? Symbol,
//     IReadOnlyList<CompactSymbolSummary> BaseTypes,
//     IReadOnlyList<CompactSymbolSummary> ImplementedInterfaces,
//     IReadOnlyList<CompactSymbolSummary> DerivedTypes,
//     ErrorInfo? Error = null);

public sealed record GetSymbolOutlineResult(SymbolDescriptor? Symbol,
    IReadOnlyList<SymbolMemberOutline> Members,
    IReadOnlyList<string> Attributes,
    ErrorInfo? Error = null);

public sealed record GetCallGraphResult(SymbolDescriptor? RootSymbol,
    IReadOnlyList<CallEdge> Edges,
    int NodeCount,
    int EdgeCount,
    ErrorInfo? Error = null);

public sealed record GetCallersResult(SymbolDescriptor? Symbol, IReadOnlyList<CallEdge> Callers, ErrorInfo? Error = null);

public sealed record GetCalleesResult(SymbolDescriptor? Symbol, IReadOnlyList<CallEdge> Callees, ErrorInfo? Error = null);

// public sealed record AddMethodResult(
//     string Status,
//     IReadOnlyList<string> ChangedFiles,
//     string TargetTypeSymbolId,
//     AddedMethodInfo? AddedMethod,
//     DiagnosticsDeltaInfo DiagnosticsDelta,
//     ErrorInfo? Error = null);

// public sealed record DeleteMethodResult(
//     string Status,
//     IReadOnlyList<string> ChangedFiles,
//     string TargetMethodSymbolId,
//     DeletedMethodInfo? DeletedMethod,
//     DiagnosticsDeltaInfo DiagnosticsDelta,
//     ErrorInfo? Error = null);

// public sealed record ReplaceMethodResult(
//     string Status,
//     IReadOnlyList<string> ChangedFiles,
//     string TargetMethodSymbolId,
//     ReplacedMethodInfo? ReplacedMethod,
//     DiagnosticsDeltaInfo DiagnosticsDelta,
//     ErrorInfo? Error = null);

// public sealed record ReplaceMethodBodyResult(
//     string Status,
//     IReadOnlyList<string> ChangedFiles,
//     string TargetMethodSymbolId,
//     ReplacedMethodBodyInfo? ReplacedMethodBody,
//     DiagnosticsDeltaInfo DiagnosticsDelta,
//     ErrorInfo? Error = null);

// public sealed record RenameSymbolResult(
//     string? RenamedSymbolId,
//     int ChangedDocumentCount,
//     IReadOnlyList<AffectedFileLocations> AffectedLocationFiles,
//     IReadOnlyList<string> ChangedFiles,
//     ErrorInfo? Error = null);

// public sealed record FormatDocumentResult(
//     string Path,
//     bool WasFormatted,
//     ErrorInfo? Error = null);

public sealed record GetCodeFixesResult(IReadOnlyList<CodeFixDescriptor> Fixes, ErrorInfo? Error = null);

public sealed record PreviewCodeFixResult(
    string FixId,
    string Title,
    IReadOnlyList<ChangedFilePreview> Changes,
    ErrorInfo? Error = null);

public sealed record ApplyCodeFixResult(
    string FixId,
    int ChangedDocumentCount,
    IReadOnlyList<string> ChangedFiles,
    ErrorInfo? Error = null);

public sealed record ExecuteCleanupResult(
    string Scope,
    IReadOnlyList<string> AppliedRuleIds,
    IReadOnlyList<string> ChangedFiles,
    IReadOnlyList<string> Warnings,
    ErrorInfo? Error = null);

public sealed record GetRefactoringsAtPositionResult(
    IReadOnlyList<RefactoringActionDescriptor> Actions,
    ErrorInfo? Error = null);

public sealed record PreviewRefactoringResult(
    string ActionId,
    string Title,
    IReadOnlyList<ChangedFilePreview> Changes,
    ErrorInfo? Error = null);

public sealed record ApplyRefactoringResult(
    string ActionId,
    int ChangedDocumentCount,
    IReadOnlyList<string> ChangedFiles,
    ErrorInfo? Error = null);

public sealed record DiscoverSolutionsResult(IReadOnlyList<string> SolutionPaths, ErrorInfo? Error = null);

public sealed record SelectSolutionResult(string? SelectedSolutionPath, ErrorInfo? Error = null);

public sealed record ReloadSolutionResult(bool Success, ErrorInfo? Error = null);

// public sealed record RunTestsResult(
//     string Outcome,
//     int? ExitCode,
//     IReadOnlyList<TestFailureGroup> FailureGroups,
//     IReadOnlyList<BuildDiagnostic>? BuildDiagnostics = null,
//     string? Summary = null,
//     ErrorInfo? Error = null,
//     TestRunCounts? Counts = null);
