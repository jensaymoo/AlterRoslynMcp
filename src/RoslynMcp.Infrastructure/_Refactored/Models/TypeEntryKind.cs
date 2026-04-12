namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Вид типа в модели Roslyn.
/// Соответствует <see cref="Microsoft.CodeAnalysis.TypeKind"/>.
/// </summary>
public enum TypeEntryKind
{
    /// <summary>Тип record (C# 9+).</summary>
    Record,
    
    /// <summary>Класс.</summary>
    Class,
    
    /// <summary>Структура (struct).</summary>
    Struct,
    
    /// <summary>Перечисление (enum).</summary>
    Enum,
    
    /// <summary>Интерфейс.</summary>
    Interface,
    
    /// <summary>Неизвестный тип.</summary>
    Unknown
}
