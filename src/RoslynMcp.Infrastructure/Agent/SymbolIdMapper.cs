using System.Collections.Concurrent;

namespace RoslynMcp.Infrastructure.Agent;

/// <summary>
/// Static mapper for converting between internal Roslyn symbol IDs and external short string IDs.
/// External IDs are short (e.g., "S0001"), token-friendly, and less error-prone for copy/paste.
/// </summary>
/// 
/// <remarks>
/// Integration points where symbol IDs flow between agent and Roslyn:
/// 
/// REQUESTS (Input - convert external ID to internal ID before passing to handler):
/// - ListMembersRequest.TypeSymbolId → ListMembersTool
/// - ListDependenciesRequest.SymbolId → ListDependenciesTool  
/// - ExplainSymbolRequest.SymbolId → ExplainSymbolTool
/// - ResolveSymbolRequest.SymbolId → ResolveSymbolTool
/// - FindSymbolRequest.SymbolId
/// - GetSignatureRequest.SymbolId
/// - FindReferencesRequest.SymbolId
/// - FindImplementationsRequest.SymbolId
/// - GetTypeHierarchyRequest.SymbolId → GetTypeHierarchyTool
/// - GetCallersRequest.SymbolId → FindCallersTool
/// - GetCalleesRequest.SymbolId → FindCalleesTool
/// - GetCallGraphRequest.SymbolId
/// - RenameSymbolRequest.SymbolId → RenameSymbolTool
/// - AddMethodRequest.TargetTypeSymbolId → AddMethodTool
/// - DeleteMethodRequest.TargetMethodSymbolId → DeleteMethodTool
/// - ReplaceMethodRequest.SymbolId, TargetMethodSymbolId → ReplaceMethodTool
/// - ReplaceMethodBodyRequest.TargetMethodSymbolId → ReplaceMethodBodyTool
/// 
/// RESPONSES (Output - convert internal ID to external ID before returning to agent):
/// - SymbolReference.SymbolId (via CodeUnderstandingExtensions.ToSymbolReference())
/// - SymbolDescriptor.SymbolId (via NavigationModelExtensions.ToSymbolDescriptor())
/// - TypeListEntry.SymbolId → ListTypesHandler
/// - MemberListEntry.SymbolId → ListMembersHandler
/// - ResolveSymbolBatchEntry.SymbolId → ResolveSymbolsBatchHandler
/// - CallEdge.FromSymbolId, CallEdge.ToSymbolId → CallGraphService
/// - MetricItem.SymbolId → AnalysisMetricsCollector
/// </remarks>
public static class SymbolIdMapper
{
    private static readonly ConcurrentDictionary<string, string> internalToExternal = new();
    private static readonly ConcurrentDictionary<string, string> externalToInternal = new();

    private static int _nextId = 0;

    extension(string id)
    {
        public string ToExternal()
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            if (internalToExternal.TryGetValue(id, out var existingId))
                return existingId;

            var newId = Interlocked.Increment(ref _nextId);
            var externalId = $"S{newId:D4}";

            internalToExternal[id] = externalId;
            externalToInternal[externalId] = id;

            return externalId;
        }

        public string ToInternal()
        {
            if (TryToInternal(id, out var internalId))
                return internalId;

            throw new KeyNotFoundException($"External ID not found in mapping: {id}");
        }

        public bool Update(string newInternalId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newInternalId))
                return false;

            if (!internalToExternal.TryGetValue(id, out var externalId))
                return false;

            internalToExternal.TryRemove(id, out _);

            internalToExternal[newInternalId] = externalId;
            externalToInternal[externalId] = newInternalId;

            return true;
        }

        public bool TryToInternal(out string? internalId)
        {
            internalId = null;

            if (string.IsNullOrEmpty(id))
                return false;

            return externalToInternal.TryGetValue(id, out internalId);
        }
    }
}