using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class SymbolAccessibilityExtensions
{
    private static SymbolAccessibility ToSymbolAccessibility(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Internal => SymbolAccessibility.Internal,
            Accessibility.Private => SymbolAccessibility.Private,
            Accessibility.Protected => SymbolAccessibility.Protected,
            Accessibility.Public => SymbolAccessibility.Public,
            Accessibility.ProtectedAndInternal => SymbolAccessibility.PrivateProtected,
            Accessibility.ProtectedOrInternal => SymbolAccessibility.ProtectedInternal,
            _ => SymbolAccessibility.NotApplicable
        };
    }

    internal static SymbolAccessibility GetSymbolAccessibility(this INamedTypeSymbol symbol)
        => ToSymbolAccessibility(symbol.DeclaredAccessibility);

    internal static SymbolAccessibility GetSymbolAccessibility(this ISymbol member)
        => ToSymbolAccessibility(member.DeclaredAccessibility);
}