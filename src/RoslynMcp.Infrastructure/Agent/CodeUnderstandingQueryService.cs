using RoslynMcp.Core;
using RoslynMcp.Core.Contracts;
using RoslynMcp.Core.Models.Agent;
using RoslynMcp.Core.Models.Analysis;
using RoslynMcp.Core.Models.Common;
using RoslynMcp.Core.Models.Navigation;
using RoslynMcp.Infrastructure.Navigation;
using RoslynMcp.Infrastructure.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace RoslynMcp.Infrastructure.Agent;

internal sealed class CodeUnderstandingQueryService
{
    private const int DefaultPageSize = 100;
    private const int MaximumPageSize = 500;

    private readonly IRoslynSolutionAccessor _solutionAccessor;
    private readonly ISolutionSessionService _solutionSessionService;
    private readonly IWorkspaceBootstrapService _workspaceBootstrapService;
    private readonly ISymbolLookupService _symbolLookupService;
    private readonly INavigationService _navigationService;

    public CodeUnderstandingQueryService(
        IRoslynSolutionAccessor solutionAccessor,
        ISolutionSessionService solutionSessionService,
        IWorkspaceBootstrapService workspaceBootstrapService,
        ISymbolLookupService symbolLookupService,
        INavigationService navigationService)
    {
        _solutionAccessor = solutionAccessor;
        _solutionSessionService = solutionSessionService;
        _workspaceBootstrapService = workspaceBootstrapService;
        _symbolLookupService = symbolLookupService;
        _navigationService = navigationService;
    }

    public async Task<(Solution? Solution, ErrorInfo? Error)> GetCurrentSolutionWithAutoBootstrapAsync(
        string noSolutionNextAction,
        string? workspaceHintPath,
        CancellationToken ct)
    {
        var (solution, error) = await _solutionAccessor.GetCurrentSolutionAsync(ct).ConfigureAwait(false);
        if (solution != null)
        {
            return (solution, null);
        }

        var discoveryRoot = ResolveDiscoveryRoot(workspaceHintPath);
        var discovered = await _solutionSessionService
            .DiscoverSolutionsAsync(new DiscoverSolutionsRequest(discoveryRoot), ct)
            .ConfigureAwait(false);

        if (discovered.Error != null || discovered.SolutionPaths.Count != 1)
        {
            return (null, AgentErrorInfo.Normalize(error, noSolutionNextAction));
        }

        var load = await _workspaceBootstrapService
            .LoadSolutionAsync(new LoadSolutionRequest(discovered.SolutionPaths[0]), ct)
            .ConfigureAwait(false);

        if (load.Error != null)
        {
            return (null, AgentErrorInfo.Normalize(load.Error, noSolutionNextAction));
        }

        var (autoLoadedSolution, autoLoadedError) = await _solutionAccessor.GetCurrentSolutionAsync(ct).ConfigureAwait(false);
        if (autoLoadedSolution == null)
        {
            return (null, AgentErrorInfo.Normalize(autoLoadedError ?? error, noSolutionNextAction));
        }

        return (autoLoadedSolution, null);
    }

    public async Task<IReadOnlyList<HotspotSummary>> BuildHotspotsAsync(
        Solution solution,
        IReadOnlyList<MetricItem> metrics,
        int hotspotCount,
        CancellationToken ct)
    {
        var ranked = metrics
            .OrderByDescending(static m => m.CyclomaticComplexity ?? 0)
            .ThenByDescending(static m => m.LineCount ?? 0)
            .ThenBy(static m => m.SymbolId, StringComparer.Ordinal)
            .Take(hotspotCount)
            .ToArray();

        var hotspots = new List<HotspotSummary>(ranked.Length);
        foreach (var metric in ranked)
        {
            var complexity = metric.CyclomaticComplexity ?? 0;
            var lineCount = metric.LineCount ?? 0;
            var score = complexity + lineCount;

            var symbol = await _symbolLookupService.ResolveSymbolAsync(metric.SymbolId, solution, ct).ConfigureAwait(false);
            var displayName = symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? metric.SymbolId;
            var (filePath, startLine, _, endLine, _) = symbol.GetSourceSpan();
            var reason = $"complexity={complexity}, lines={lineCount}";
            if (string.IsNullOrWhiteSpace(filePath))
            {
                reason += ", location=unknown";
            }

            hotspots.Add(new HotspotSummary(
                Label: displayName,
                Path: filePath,
                Reason: reason,
                Score: score,
                SymbolId: metric.SymbolId,
                DisplayName: displayName,
                FilePath: filePath,
                StartLine: startLine,
                EndLine: endLine,
                Complexity: complexity,
                LineCount: lineCount));
        }

        return hotspots
            .OrderByDescending(static h => h.Score)
            .ThenBy(static h => h.SymbolId, StringComparer.Ordinal)
            .ToArray();
    }

    public async Task<GetSymbolAtPositionResult> ResolveSymbolAtRequestAsync(
        string? symbolId,
        string? path,
        int? line,
        int? column,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(symbolId))
        {
            var find = await _navigationService.FindSymbolAsync(new FindSymbolRequest(symbolId), ct).ConfigureAwait(false);
            return new GetSymbolAtPositionResult(find.Symbol, find.Error);
        }

        if (!string.IsNullOrWhiteSpace(path) && line.HasValue && column.HasValue)
        {
            return await _navigationService.GetSymbolAtPositionAsync(
                new GetSymbolAtPositionRequest(path, line.Value, column.Value),
                ct).ConfigureAwait(false);
        }

        return new GetSymbolAtPositionResult(
            null,
            AgentErrorInfo.Create(
                ErrorCodes.InvalidInput,
                "Provide symbolId or path/line/column.",
                "Call explain_symbol with a symbolId or source position."));
    }

    public async Task<(INamedTypeSymbol? Symbol, ErrorInfo? Error)> ResolveTypeSymbolAsync(
        ListMembersRequest request,
        Solution solution,
        CancellationToken ct)
    {
        var hasExplicitTypeSymbolId = !string.IsNullOrWhiteSpace(request.TypeSymbolId);
        ISymbol? symbol;

        if (hasExplicitTypeSymbolId)
        {
            symbol = await _symbolLookupService.ResolveSymbolAsync(request.TypeSymbolId!, solution, ct).ConfigureAwait(false);
            if (symbol == null)
            {
                return (null,
                    AgentErrorInfo.Create(
                        ErrorCodes.InvalidInput,
                        $"typeSymbolId '{request.TypeSymbolId}' could not be resolved.",
                        "Call list_types first to select a valid type symbolId.",
                        ("field", "typeSymbolId"),
                        ("provided", request.TypeSymbolId),
                        ("expected", "type symbolId returned by list_types")));
            }

            if (symbol is not INamedTypeSymbol namedType)
            {
                return (null,
                    AgentErrorInfo.Create(
                        ErrorCodes.InvalidInput,
                        "typeSymbolId must resolve to a type symbol.",
                        "Call list_types and pass a type symbolId, not a member symbolId.",
                        ("field", "typeSymbolId"),
                        ("provided", request.TypeSymbolId),
                        ("expected", "type symbolId")));
            }

            return (namedType, null);
        }

        if (!string.IsNullOrWhiteSpace(request.Path) && request.Line.HasValue && request.Column.HasValue)
        {
            symbol = await _symbolLookupService
                .GetSymbolAtPositionAsync(solution, request.Path!, request.Line.Value, request.Column.Value, ct)
                .ConfigureAwait(false);
            if (symbol == null)
            {
                return (null,
                    AgentErrorInfo.Create(
                        ErrorCodes.SymbolNotFound,
                        "No symbol found at the provided source position.",
                        "Call list_members with a valid typeSymbolId from list_types, or provide a valid source position."));
            }
        }
        else
        {
            return (null,
                AgentErrorInfo.Create(
                    ErrorCodes.InvalidInput,
                    "Provide typeSymbolId or path/line/column.",
                    "Call list_members with a typeSymbolId from list_types, or provide a source position."));
        }

        var typeSymbol = symbol switch
        {
            INamedTypeSymbol namedType => namedType,
            _ => symbol.ContainingType
        };

        if (typeSymbol == null)
        {
            return (null,
                AgentErrorInfo.Create(
                    ErrorCodes.InvalidInput,
                    "Resolved symbol is not a type and has no containing type.",
                    "Call list_members with a symbolId that resolves to a type declaration.",
                    ("field", "typeSymbolId")));
        }

        return (typeSymbol, null);
    }

    public static async Task<ResolveSymbolCandidate[]> ResolveByQualifiedNameAsync(
        string qualifiedName,
        IReadOnlyList<Project> projects,
        CancellationToken ct)
    {
        var normalizedQualifiedName = qualifiedName.NormalizeQualifiedName();
        var shortName = normalizedQualifiedName.Split('.').LastOrDefault();
        if (string.IsNullOrWhiteSpace(shortName))
        {
            return Array.Empty<ResolveSymbolCandidate>();
        }

        var candidates = new List<(ISymbol Symbol, string ProjectName)>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var project in projects.OrderBy(static p => p.Name, StringComparer.Ordinal))
        {
            var symbols = await SymbolFinder.FindDeclarationsAsync(
                    project,
                    shortName,
                    ignoreCase: false,
                    filter: SymbolFilter.TypeAndMember,
                    cancellationToken: ct)
                .ConfigureAwait(false);

            foreach (var symbol in symbols)
            {
                var symbolId = SymbolIdentity.CreateId(symbol);
                var candidateKey = $"{project.Id.Id:N}|{symbolId}";

                if (!seen.Add(candidateKey))
                {
                    continue;
                }

                candidates.Add((symbol, project.Name));
            }
        }

        var strictMatches = candidates
            .Where(match => match.Symbol.MatchesQualifiedName(normalizedQualifiedName))
            .ToArray();
        if (strictMatches.Length > 0)
        {
            return OrderResolveSymbolCandidates(strictMatches, shortName);
        }

        if (!normalizedQualifiedName.LooksLikeShortNameQuery())
        {
            return Array.Empty<ResolveSymbolCandidate>();
        }

        var caseSensitiveMatches = candidates
            .Where(match => string.Equals(match.Symbol.Name, shortName, StringComparison.Ordinal))
            .ToArray();

        var shortNameMatches = caseSensitiveMatches.Length > 0
            ? caseSensitiveMatches
            : candidates
                .Where(match => string.Equals(match.Symbol.Name, shortName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        return OrderResolveSymbolCandidates(shortNameMatches, shortName);
    }

    public static ResolveSymbolCandidate[] OrderResolveSymbolCandidates(
        IReadOnlyList<(ISymbol Symbol, string ProjectName)> matches,
        string shortName)
    {
        return matches
            .OrderByDescending(match => string.Equals(match.Symbol.Name, shortName, StringComparison.Ordinal))
            .ThenBy(match => GetResolveSymbolKindPriority(match.Symbol))
            .ThenBy(match => match.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), StringComparer.Ordinal)
            .ThenBy(match => match.ProjectName, StringComparer.Ordinal)
            .ThenBy(match => SymbolIdentity.CreateId(match.Symbol), StringComparer.Ordinal)
            .Select(match =>
            {
                var symbolId = SymbolIdentity.CreateId(match.Symbol);
                var (filePath, line, column) = match.Symbol.GetDeclarationPosition();
                return new ResolveSymbolCandidate(
                    symbolId,
                    match.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    match.Symbol.Kind.ToString(),
                    filePath,
                    line,
                    column,
                    match.ProjectName);
            })
            .ToArray();
    }

    public static int GetResolveSymbolKindPriority(ISymbol symbol)
        => symbol is INamedTypeSymbol ? 0 : 1;

    public static IReadOnlyList<Project> ResolveProjectSelector(
        Solution solution,
        string? projectPath,
        string? projectName,
        string? projectId,
        bool selectorRequired,
        string toolName,
        out ErrorInfo? error)
    {
        var normalizedPath = NormalizeOptional(projectPath);
        var normalizedName = NormalizeOptional(projectName);
        var normalizedId = NormalizeOptional(projectId);

        if (normalizedPath == null && normalizedName == null && normalizedId == null)
        {
            if (selectorRequired)
            {
                error = AgentErrorInfo.Create(
                    ErrorCodes.InvalidInput,
                    "A project selector is required. Provide projectPath, projectName, or projectId.",
                    $"Call {toolName} with one project selector from load_solution results.",
                    ("field", "project selector"),
                    ("expected", "projectPath|projectName|projectId"));
                return Array.Empty<Project>();
            }

            error = null;
            return solution.Projects.OrderBy(static p => p.Name, StringComparer.Ordinal).ToArray();
        }

        var matches = solution.Projects
            .Where(project => normalizedPath == null || NavigationModelUtilities.MatchesByNormalizedPath(project.FilePath, normalizedPath))
            .Where(project => normalizedName == null || string.Equals(project.Name, normalizedName, StringComparison.OrdinalIgnoreCase))
            .Where(project => normalizedId == null || string.Equals(project.Id.Id.ToString(), normalizedId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static project => project.Name, StringComparer.Ordinal)
            .ToArray();

        if (matches.Length == 0)
        {
            var provided = string.Join(", ",
                new[]
                {
                    normalizedPath is null ? null : $"projectPath={normalizedPath}",
                    normalizedName is null ? null : $"projectName={normalizedName}",
                    normalizedId is null ? null : $"projectId={normalizedId}"
                }.Where(static value => value != null)!);

            error = AgentErrorInfo.Create(
                ErrorCodes.InvalidInput,
                "Project selector did not match any loaded project.",
                "Use load_solution output to provide an exact projectPath, projectName, or projectId.",
                ("field", "project selector"),
                ("provided", provided));
            return Array.Empty<Project>();
        }

        if (matches.Length > 1)
        {
            var names = string.Join(", ", matches.Select(static project => project.Name));
            error = AgentErrorInfo.Create(
                ErrorCodes.InvalidInput,
                "Project selector is ambiguous and matched multiple projects.",
                "Provide projectPath or projectId to uniquely identify the project.",
                ("field", "project selector"),
                ("matches", names));
            return Array.Empty<Project>();
        }

        error = null;
        return matches;
    }

    public static string NormalizeProfile(string? profile)
    {
        var normalized = string.IsNullOrWhiteSpace(profile) ? "standard" : profile.Trim().ToLowerInvariant();
        return normalized is "quick" or "standard" or "deep" ? normalized : "standard";
    }

    public static (int Offset, int Limit) NormalizePaging(int? offset, int? limit)
    {
        var normalizedOffset = Math.Max(offset ?? 0, 0);
        var normalizedLimit = limit.HasValue
            ? Math.Clamp(limit.Value, 0, MaximumPageSize)
            : DefaultPageSize;
        return (normalizedOffset, normalizedLimit);
    }

    public static bool TryNormalizeAccessibility(string? accessibility, out string? normalized)
    {
        var value = NormalizeOptional(accessibility);
        if (value == null)
        {
            normalized = null;
            return true;
        }

        normalized = value.Replace('-', '_').ToLowerInvariant();
        if (normalized is "public" or "internal" or "protected" or "private" or "protected_internal" or "private_protected")
        {
            return true;
        }

        normalized = null;
        return false;
    }

    public static bool TryNormalizeTypeKind(string? kind, out string? normalized)
    {
        normalized = NormalizeOptional(kind)?.ToLowerInvariant();
        if (normalized == null)
        {
            return true;
        }

        if (normalized is "class" or "record" or "interface" or "enum" or "struct")
        {
            return true;
        }

        normalized = null;
        return false;
    }

    public static bool TryNormalizeMemberKind(string? kind, out string? normalized)
    {
        normalized = NormalizeOptional(kind)?.ToLowerInvariant();
        if (normalized == null)
        {
            return true;
        }

        if (normalized is "method" or "property" or "field" or "event" or "ctor")
        {
            return true;
        }

        normalized = null;
        return false;
    }

    public static bool TryNormalizeBinding(string? binding, out string? normalized)
    {
        normalized = NormalizeOptional(binding)?.ToLowerInvariant();
        if (normalized == null)
        {
            return true;
        }

        if (normalized is "static" or "instance")
        {
            return true;
        }

        normalized = null;
        return false;
    }

    public static bool TryNormalizeDependencyDirection(string? direction, out string normalized)
    {
        normalized = NormalizeOptional(direction)?.ToLowerInvariant() ?? "both";
        return normalized is "outgoing" or "incoming" or "both";
    }

    public static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static string ResolveDiscoveryRoot(string? workspaceHintPath)
    {
        var normalizedHint = NormalizeOptional(workspaceHintPath);
        if (normalizedHint == null)
        {
            return Directory.GetCurrentDirectory();
        }

        if (normalizedHint.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
            || normalizedHint.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            var directory = Path.GetDirectoryName(normalizedHint);
            if (string.IsNullOrWhiteSpace(directory))
            {
                return Directory.GetCurrentDirectory();
            }

            if (normalizedHint.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var parent = Path.GetDirectoryName(directory);
                if (!string.IsNullOrWhiteSpace(parent))
                {
                    return parent;
                }
            }

            return directory;
        }

        return normalizedHint;
    }
}
