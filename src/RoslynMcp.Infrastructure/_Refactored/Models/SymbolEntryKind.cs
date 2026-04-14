namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Вид символа (тип или член).
/// </summary>
public enum SymbolEntryKind
{
    // Типы
    Class,
    Struct,
    Interface,
    Enum,
    Record,
    Delegate,

    // Члены
    Method,
    Property,
    Field,
    Event,
    Constructor,

    // Прочее
    Namespace,
    Unknown
}
