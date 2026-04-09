using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SymbolDisplay;

namespace RoslynMcp.Infrastructure._Refactored;

public class MemberExtractor : IMemberExtractor
{
    public string GetDisplayName(ISymbol member)
    {
        if (member.Kind == SymbolKind.Method && member is IMethodSymbol { MethodKind: MethodKind.Constructor })
            return ((IMethodSymbol)member).ContainingType.Name;

        return member.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    public string GetSignature(ISymbol member)
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

    public MemberEntryKind GetKind(ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => MemberEntryKind.Constructor,
            IMethodSymbol => MemberEntryKind.Method,
            IPropertySymbol => MemberEntryKind.Property,
            IFieldSymbol => MemberEntryKind.Field,
            IEventSymbol => MemberEntryKind.Event,
            _ => MemberEntryKind.Method
        };
    }

    public bool GetIsStatic(ISymbol member)
    {
        return member.IsStatic;
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
