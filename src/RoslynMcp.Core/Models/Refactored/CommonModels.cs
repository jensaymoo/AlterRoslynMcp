namespace RoslynMcp.Core.Models;

public enum RiskLevel
{
    Low,
    Medium,
    High
}

public static class SourceBiases
{
    public const string Handwritten = "handwritten";
    public const string Generated = "generated";
    public const string Mixed = "mixed";
    public const string Unknown = "unknown";
}

public static class ResultCompletenessStates
{
    public const string Complete = "complete";
    public const string Partial = "partial";
    public const string Degraded = "degraded";
}

public static class WorkspaceReadinessStates
{
    public const string Ready = "ready";
    public const string DegradedMissingArtifacts = "degraded_missing_artifacts";
    public const string DegradedRestoreRecommended = "degraded_restore_recommended";
}

public static class CodeSmellReviewModes
{
    public const string Default = "default";
    public const string Conservative = "conservative";
}

public static class CodeSmellReviewKinds
{
    public const string StyleSuggestion = "style_suggestion";
    public const string CodeFixHint = "code_fix_hint";
    public const string ReviewConcern = "review_concern";
}

public static class FlowEvidenceKinds
{
    public const string DirectStatic = "direct_static";
    public const string PossibleTarget = "possible_target";
}

public static class FlowUncertaintyCategories
{
    public const string InterfaceDispatch = "interface_dispatch";
    public const string PolymorphicInference = "polymorphic_inference";
    public const string ReflectionBlindspot = "reflection_blindspot";
    public const string DynamicUnresolved = "dynamic_unresolved";
    public const string UnresolvedProject = "unresolved_project";
    public const string ProjectInferenceDegraded = "project_inference_degraded";
}

public static class AnalysisScopes
{
    public const string Document = "document";
    public const string Project = "project";
    public const string Solution = "solution";
}

public static class SymbolSearchScopes
{
    public const string Document = "document";
    public const string Project = "project";
    public const string Solution = "solution";
}

public static class ReferenceScopes
{
    public const string Document = "document";
    public const string Project = "project";
    public const string Solution = "solution";
}

public static class CallGraphDirections
{
    public const string Incoming = "incoming";
    public const string Outgoing = "outgoing";
    public const string Both = "both";
}

public static class RunTestOutcomes
{
    public const string Passed = "passed";
    public const string TestFailures = "test_failures";
    public const string BuildFailed = "build_failed";
    public const string InfrastructureError = "infrastructure_error";
    public const string Cancelled = "cancelled";
}
