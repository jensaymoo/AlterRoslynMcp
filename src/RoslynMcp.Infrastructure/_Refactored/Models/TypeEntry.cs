using Microsoft.CodeAnalysis;
using System.Linq;

namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Представляет информацию о типе (класс, структура, интерфейс, перечисление, record),
/// извлечённую из символа Roslyn <see cref="Microsoft.CodeAnalysis.INamedTypeSymbol"/>.
/// </summary>
public sealed class TypeEntry
{
    /// <summary>
    /// Имя символа типа.
    /// </summary>
    public string SymbolName { get; init; }
    
    /// <summary>
    /// Вид типа (класс, структура, интерфейс, перечисление, record).
    /// </summary>
    public TypeEntryKind Kind { get; init; }
    
    /// <summary>
    /// Область видимости типа (public, internal, private и т.д.).
    /// </summary>
    public SymbolAccessibility Accessibility { get; init; }
    
    /// <summary>
    /// Пространство имён, в котором объявлен тип.
    /// </summary>
    public string? Namespace { get; init; }
    
    /// <summary>
    /// Базовые типы (базовый класс и реализуемые интерфейсы).
    /// </summary>
    public IEnumerable<TypeEntry>? BaseTypes { get; init; }
    
    /// <summary>
    /// Расположение исходного кода типа в файлах.
    /// </summary>
    public IEnumerable<SourceLocation> Location { get; init; }
    
    /// <summary>
    /// Краткое описание из документации (XML-комментариев).
    /// </summary>
    public string? Summary { get; init; }
    
    /// <summary>
    /// Имя проекта, содержащего тип.
    /// </summary>
    public string ProjectName { get; init; }
    
    /// <summary>
    /// Путь к файлу проекта.
    /// </summary>
    public string? ProjectPath { get; init; }

    /// <summary>
    /// Создаёт экземпляр <see cref="TypeEntry"/> на основе символа типа Roslyn.
    /// </summary>
    /// <param name="symbol">Символ типа из <see cref="Microsoft.CodeAnalysis.INamedTypeSymbol"/>.</param>
    /// <param name="project">Проект Roslyn, содержащий тип <see cref="Microsoft.CodeAnalysis.Project"/>.</param>
    public TypeEntry(Microsoft.CodeAnalysis.INamedTypeSymbol symbol, Microsoft.CodeAnalysis.Project project)
    {
        SymbolName = symbol.GetSymbolName();
        Kind = symbol.GetTypeEntryKind();
        Accessibility = symbol.GetSymbolAccessibility();
        Namespace = symbol.ContainingNamespace.GetSymbolNameOrNull();
        Location = symbol.Locations.AsSourceLocations();
        Summary = symbol.GetDocumentationSummary();

        ProjectName = project.Name;
        ProjectPath = project.FilePath;

        BaseTypes = GetDirectBaseTypes(symbol, project);
    }

    private static IEnumerable<TypeEntry>? GetDirectBaseTypes(INamedTypeSymbol type, Project project)
    {
        if (type.TypeKind == TypeKind.Enum || !project.TryGetCompilation(out var compilation))
            return null;

        var entries = GetDirectBaseSymbols(type)
            .Select(t => CreateTypeEntryOrNull(t, project, compilation))
            .OfType<TypeEntry>()
            .ToList();

        return entries;
    }

    private static IEnumerable<INamedTypeSymbol> GetDirectBaseSymbols(INamedTypeSymbol type)
    {
        if (type.BaseType is { IsImplicitlyDeclared: false } baseType)
            yield return baseType;

        foreach (var iface in type.Interfaces.Where(i => !i.IsImplicitlyDeclared))
            yield return iface;
    }

    private static TypeEntry? CreateTypeEntryOrNull(INamedTypeSymbol symbol, Project project, Compilation compilation)
    {
        var syntaxRef = symbol.DeclaringSyntaxReferences.FirstOrDefault();
        var isFromProject = syntaxRef is not null && compilation.SyntaxTrees.Contains(syntaxRef.SyntaxTree);

        return isFromProject ? new TypeEntry(symbol, project) : null;
    }
}
