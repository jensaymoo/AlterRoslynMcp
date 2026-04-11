using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class SignatureExtensions
{
    internal static string GetSignature(this ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } constructor
                => $"{constructor.ContainingType.Name}({FormatParameters(constructor.Parameters)})",
            IMethodSymbol method => FormatMethodSignature(method),
            IPropertySymbol property => $"{property.Type.ToDisplayString()} {property.Name}",
            IFieldSymbol field => $"{field.Type.ToDisplayString()} {field.Name}",
            IEventSymbol @event => $"{@event.Type.ToDisplayString()} {@event.Name}",
            _ => member.Name
        };
    }

    private static string FormatMethodSignature(IMethodSymbol method)
    {
        var returnType = method.ReturnType.ToDisplayString();
        var parameters = FormatParameters(method.Parameters);
        return $"{returnType} {method.Name}({parameters})";
    }

    private static string FormatParameters(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(", ", parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
    }
}