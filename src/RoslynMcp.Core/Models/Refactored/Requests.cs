namespace RoslynMcp.Core.Models;

// Per request classes moved to subdirectories
// public sealed record LoadSolutionRequest(string? SolutionHintPath = null);

// public sealed record UnderstandProjectsRequest(string? Profile = null);

// public sealed record ListTypesRequest(
//     string? ProjectPath = null,
//     string? ProjectName = null,
//     string? ProjectId = null,
//     string? NamespacePrefix = null,
//     string? Kind = null,
//     string? Accessibility = null,
//     bool IncludeSummary = false,
//     bool IncludeMembers = false,
//     int? Limit = null,
//     int? Offset = null);

// public sealed record ListMembersRequest(
//     string? TypeSymbolId = null,
//     string? Path = null,
//     int? Line = null,
//     int? Column = null,
//     string? Kind = null,
//     string? Accessibility = null,
//     string? Binding = null,
//     bool IncludeInherited = false,
//     int? Limit = null,
//     int? Offset = null);

// public sealed record ResolveSymbolRequest(
//     string? SymbolId = null,
//     string? Path = null,
//     int? Line = null,
//     int? Column = null,
//     string? QualifiedName = null,
//     string? ProjectPath = null,
//     string? ProjectName = null,
//     string? ProjectId = null);

// public sealed record ResolveSymbolBatchEntry(
//     string? SymbolId = null,
//     string? Path = null,
//     int? Line = null,
//     int? Column = null,
//     string? QualifiedName = null,
//     string? ProjectPath = null,
//     string? ProjectName = null,
//     string? ProjectId = null,
//     string? Label = null);

// public sealed record ResolveSymbolsBatchRequest(IReadOnlyList<ResolveSymbolBatchEntry> Entries);

// public sealed record ExplainSymbolRequest(string? SymbolId = null, string? Path = null, int? Line = null, int? Column = null);

// public sealed record TraceFlowRequest(
//     string? SymbolId = null,
//     string? Path = null,
//     int? Line = null,
//     int? Column = null,
//     string? Direction = null,
//     int? Depth = null,
//     bool IncludePossibleTargets = false);

// public sealed record FindCodeSmellsRequest(
//     string Path,
//     int? MaxFindings = null,
//     IReadOnlyList<string>? RiskLevels = null,
//     IReadOnlyList<string>? Categories = null,
//     string? ReviewMode = null);

public sealed record AnalyzeSolutionRequest();

public sealed record AnalyzeScopeRequest(string Scope, string? Path = null);

public sealed record GetCodeMetricsRequest();

public sealed record FindSymbolRequest(string SymbolId);

public sealed record GetSymbolAtPositionRequest(string Path, int Line, int Column);

public sealed record SearchSymbolsRequest(string Query, int? Limit = null, int? Offset = null);

public sealed record SearchSymbolsScopedRequest(
    string Query,
    string Scope,
    string? Path = null,
    string? Kind = null,
    string? Accessibility = null,
    int? Limit = null,
    int? Offset = null);

public sealed record GetSignatureRequest(string SymbolId);

public sealed record FindReferencesRequest(string SymbolId);

// public sealed record FindReferencesScopedRequest(string SymbolId, string Scope, string? Path = null);

// public sealed record FindImplementationsRequest(string SymbolId);

// public sealed record GetTypeHierarchyRequest(string SymbolId, bool? IncludeTransitive = null, int? MaxDerived = null);

public sealed record GetSymbolOutlineRequest(string SymbolId, int? Depth = null);

public sealed record GetCallersRequest(string SymbolId, int? MaxDepth = null);

public sealed record GetCalleesRequest(string SymbolId, int? MaxDepth = null);

public sealed record GetCallGraphRequest(string SymbolId, string Direction, int? MaxDepth = null);

// public sealed record AddMethodRequest(
//     string TargetTypeSymbolId,
//     MethodInsertionSpec Method);

// public sealed record DeleteMethodRequest(string TargetMethodSymbolId);

// public sealed record ReplaceMethodRequest(
//     string TargetMethodSymbolId,
//     MethodInsertionSpec Method);

// public sealed record ReplaceMethodBodyRequest(
//     string TargetMethodSymbolId,
//     string Body);

// public sealed record RenameSymbolRequest(string SymbolId, string NewName);

// public sealed record FormatDocumentRequest(string Path);

public sealed record GetCodeFixesRequest(
    string Scope,
    string? Path = null,
    IReadOnlyList<string>? DiagnosticIds = null,
    string? Category = null);

public sealed record PreviewCodeFixRequest(string FixId);

public sealed record ApplyCodeFixRequest(string FixId);

public sealed record ExecuteCleanupRequest(
    string Scope,
    string? Path = null,
    string? PolicyProfile = null,
    int? ExpectedWorkspaceVersion = null);

public sealed record GetRefactoringsAtPositionRequest(
    string Path,
    int Line,
    int Column,
    int? SelectionStart = null,
    int? SelectionLength = null,
    string? PolicyProfile = null);

public sealed record PreviewRefactoringRequest(string ActionId);

public sealed record ApplyRefactoringRequest(string ActionId);

public sealed record DiscoverSolutionsRequest(string WorkspaceRoot);

public sealed record SelectSolutionRequest(string SolutionPath);

public sealed record ReloadSolutionRequest();

// public sealed record RunTestsRequest(string? Target = null, string? Filter = null);
