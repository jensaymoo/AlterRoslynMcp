using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Результат резолва символа по его идентификатору.
/// </summary>
public sealed class ResolvedSymbolEntry
{
    /// <summary>
    /// Уникальный идентификатор символа (documentation comment ID).
    /// </summary>
    public string SymbolId { get; init; }

    /// <summary>
    /// Отображаемое имя символа.
    /// </summary>
    public string DisplayName { get; init; }

    /// <summary>
    /// Вид символа (класс, метод, свойство и т.д.).
    /// </summary>
    public SymbolEntryKind Kind { get; init; }

    /// <summary>
    /// Расположение объявления символа в исходном коде.
    /// </summary>
    public IEnumerable<SourceLocation> Location { get; init; }

    /// <summary>
    /// Имя проекта, содержащего символ.
    /// </summary>
    public string ProjectName { get; init; }

    /// <summary>
    /// Создаёт экземпляр <see cref="ResolvedSymbolEntry"/> на основе символа Roslyn.
    /// </summary>
    /// <param name="symbol">Символ из Roslyn.</param>
    /// <param name="project">Проект, содержащий символ.</param>
    public ResolvedSymbolEntry(ISymbol symbol, Project project)
    {
        SymbolId = symbol.GetSymbolId();
        DisplayName = symbol.GetSymbolName();
        Kind = symbol.GetSymbolEntryKind();
        Location = symbol.Locations.AsSourceLocations();
        ProjectName = project.Name;
    }
}
