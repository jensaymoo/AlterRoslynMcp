using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class SymbolNameExtensions
{
    private static readonly SymbolDisplayFormat Format = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
    );

    internal static string GetSymbolName(this INamedTypeSymbol symbol)
        => symbol.ToDisplayString(Format);

    internal static string GetSymbolName(this INamespaceSymbol symbol)
        => symbol.ToDisplayString(Format);
    
    internal static string? GetSymbolNameOrNull(this INamespaceSymbol symbol)
        => symbol.IsGlobalNamespace ? null : symbol.GetSymbolName();

    internal static string GetSymbolName(this ISymbol member) =>
        member is IMethodSymbol { MethodKind: MethodKind.Constructor } ctor
            ? ctor.ContainingType.Name
            : member.ToDisplayString(Format);

    internal static string GetSymbolId(this INamedTypeSymbol symbol)
        => symbol.GetDocumentationCommentId()
           ?? throw new InvalidOperationException($"Cannot get symbol ID for type '{symbol.GetSymbolName()}'");

    internal static string GetSymbolId(this ISymbol symbol)
        => symbol.GetDocumentationCommentId()
           ?? throw new InvalidOperationException($"Cannot get symbol ID for symbol '{symbol.GetSymbolName()}'");
}