namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Вид члена типа.
/// </summary>
public enum MemberEntryKind
{
    /// <summary>Метод.</summary>
    Method,
    
    /// <summary>Свойство (property).</summary>
    Property,
    
    /// <summary>Поле (field).</summary>
    Field,
    
    /// <summary>Событие (event).</summary>
    Event,
    
    /// <summary>Конструктор.</summary>
    Constructor
}
