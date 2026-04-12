namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Представляет местоположение в исходном коде (номер строки и столбца).
/// </summary>
public sealed class SourceLocation
{
    /// <summary>
    /// Путь к файлу с исходным кодом.
    /// </summary>
    public required string FilePath { get; init; }
    
    /// <summary>
    /// Номер строки (1-based).
    /// </summary>
    public required int Line { get; init; }
    
    /// <summary>
    /// Номер столбца (1-based).
    /// </summary>
    public required int Column { get; init; }
}