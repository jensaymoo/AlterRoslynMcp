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
        
        return results.FirstOrDefault(r => r != null) 
               ?? throw new TypeEntryNotFoundException($"Type '{symbolName}' not found in solution");
    }

    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolName, Project project, CancellationToken ct = default)
    {
        if (await project.GetCompilationAsync(ct) is not { } compilation)
            return null;

        return compilation.GlobalNamespace
                   .EnumerateTypes(includeGenerated: false)
                   .FirstOrDefault(type =>
                       type.DeclaringSyntaxReferences.Any() &&
                       type.GetSymbolName() == symbolName)
               ?? throw new TypeEntryNotFoundException($"Type '{symbolName}' not found in project '{project.Name}'");
    }
}
