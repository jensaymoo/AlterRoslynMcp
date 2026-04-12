using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class TypeResolverService : ITypeResolverService
{
    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolName, Solution solution, CancellationToken ct = default)
    {
        var results = await Task.WhenAll(
            solution.Projects
                .Select(prj => GetNamedTypeAsync(symbolName, prj, ct))
            );
        
        return results.FirstOrDefault(r => r != null);
    }

    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolName, Project project, CancellationToken ct = default)
    {
        var compilation = await project.GetCompilationAsync(ct);
        if (compilation == null)
        {
            return null;
        }

        return compilation.GlobalNamespace
            .EnumerateTypes(includeGenerated: false)
            .FirstOrDefault(type =>
                type.DeclaringSyntaxReferences.Any() &&
                type.GetSymbolName() == symbolName);
    }
}
