namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Область видимости символа.
/// Соответствует <see cref="Microsoft.CodeAnalysis.Accessibility"/>.
/// </summary>
public enum SymbolAccessibility
{
    /// <summary>Доступен везде.</summary>
    Public,
    
    /// <summary>Доступен только внутри текущей сборки.</summary>
    Internal,
    
    /// <summary>Доступен в классе и производных классах.</summary>
    Protected,
    
    /// <summary>Доступен только внутри текущего класса.</summary>
    Private,
    
    /// <summary>Доступен в текущей сборке или производных классах.</summary>
    ProtectedInternal,
    
    /// <summary>Доступен только в текущей сборке внутри содержащего класса.</summary>
    PrivateProtected,
    
    /// <summary>Область видимость не применима (например, для файловых типов).</summary>
    NotApplicable
}
