using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class DocumnetationExtention
{
    internal static string? GetDocumentationSummary(this ISymbol symbol)
    {
        var doc = symbol.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(doc) ? null : doc;
    }
    
    internal static string? GetDocumentationSummary(this INamedTypeSymbol symbol)
    {
        var doc = symbol.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(doc) ? null : doc;
    }
}