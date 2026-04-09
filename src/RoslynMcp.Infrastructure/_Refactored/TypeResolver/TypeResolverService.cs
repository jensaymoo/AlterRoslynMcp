using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SymbolDisplay;

namespace RoslynMcp.Infrastructure._Refactored;

public class TypeResolverService : ITypeResolverService
{
    public string GetDisplayName(INamedTypeSymbol namedType)
    {
        return namedType.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining));
    }

    public string GetDisplayNamespace(INamedTypeSymbol namedType)
    {
        return namedType.ContainingNamespace.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining));
    }

    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string displayName, Solution solution)
    {
        foreach (var project in solution.Projects)
        {
            var result = await GetNamedTypeAsync(displayName, project);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string displayName, Project project)
    {
        var compilation = await project.GetCompilationAsync(CancellationToken.None);
        if (compilation == null)
        {
            return null;
        }

        return compilation.GlobalNamespace
            .EnumerateTypes(includeGenerated: false)
            .FirstOrDefault(type =>
                type.DeclaringSyntaxReferences.Any() &&
                GetDisplayName(type) == displayName);
    }
}
