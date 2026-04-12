using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoslynMcp.Infrastructure._Refactored;

/// <summary>
/// Представляет информацию о члене типа (методе, свойстве, поле, событии или конструкторе),
/// извлечённую из символа Roslyn <see cref="Microsoft.CodeAnalysis.ISymbol"/>.
/// </summary>
public sealed class MemberEntry
{
    /// <summary>
    /// Имя символа члена.
    /// </summary>
    public string SymbolName { get; init; }
    
    /// <summary>
    /// Сигнатура члена в строковом представлении.
    /// </summary>
    public string Signature { get; init; }
    
    /// <summary>
    /// Вид члена (метод, свойство, поле, событие, конструктор).
    /// </summary>
    public MemberEntryKind Kind { get; init; }
    
    /// <summary>
    /// Область видимости символа (public, private, protected и т.д.).
    /// </summary>
    public SymbolAccessibility Accessibility { get; init; }
    
    /// <summary>
    /// Расположение исходного кода члена в файлах.
    /// </summary>
    public IEnumerable<SourceLocation>? Location { get; init; }
    
    /// <summary>
    /// Краткое описание из документации (XML-комментариев).
    /// </summary>
    public string? Summary { get; init; }
    
    /// <summary>
    /// Является ли член статическим.
    /// </summary>
    public bool IsStatic { get; init; }
    
    /// <summary>
    /// Унаследован ли член от базового класса.
    /// </summary>
    public bool IsInherited { get; init; }

    /// <summary>
    /// Является ли член виртуальным.
    /// </summary>
    public bool IsVirtual { get; init; }
    
    /// <summary>
    /// Является ли член переопределением.
    /// </summary>
    public bool IsOverride { get; init; }
    
    /// <summary>
    /// Является ли член абстрактным.
    /// </summary>
    public bool IsAbstract { get; init; }
    
    /// <summary>
    /// Является ли член запечатанным (sealed).
    /// </summary>
    public bool IsSealed { get; init; }
    
    /// <summary>
    /// Объявлен ли член как extern (внешняя библиотека).
    /// </summary>
    public bool IsExtern { get; init; }

    /// <summary>
    /// Создаёт экземпляр <see cref="MemberEntry"/> на основе символа Roslyn.
    /// </summary>
    /// <param name="member">Символ члена типа из <see cref="Microsoft.CodeAnalysis.ISymbol"/>.</param>
    /// <param name="type">Тип, из которого извлекается член <see cref="Microsoft.CodeAnalysis.INamedTypeSymbol"/>.</param>
    public MemberEntry(ISymbol member, INamedTypeSymbol type)
    {
        SymbolName = member.GetSymbolName();
        Signature = member.GetSignature();
        Kind = member.GetMemberEntryKind();
        Accessibility = member.GetSymbolAccessibility();

        Location = member.Locations.AsSourceLocations();
        Summary = member.GetDocumentationSummary();
        
        IsStatic = member.IsStatic;
        IsInherited = IsInheritedFrom(member, type);

        IsVirtual = member.IsVirtual;
        IsOverride = member.IsOverride;
        IsAbstract = member.IsAbstract;
        IsSealed = member.IsSealed;
        IsExtern = member.IsExtern;
    }
    
    private static bool IsInheritedFrom(ISymbol member, INamedTypeSymbol sourceType)
        => member.ContainingType != null 
           && !SymbolEqualityComparer.Default.Equals(member.ContainingType, sourceType);
}
