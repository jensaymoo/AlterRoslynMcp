using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class EntryKindExtensions
{
    internal static TypeEntryKind GetTypeEntryKind(this INamedTypeSymbol symbol)
    {
        return symbol switch
        {
            { IsRecord: true } => TypeEntryKind.Record,
            { TypeKind: TypeKind.Class } => TypeEntryKind.Class,
            { TypeKind: TypeKind.Interface } => TypeEntryKind.Interface,
            { TypeKind: TypeKind.Enum } => TypeEntryKind.Enum,
            { TypeKind: TypeKind.Struct } => TypeEntryKind.Struct,
            _ => TypeEntryKind.Unknown
        };
    }

    internal static MemberEntryKind GetMemberEntryKind(this ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } 
                => MemberEntryKind.Constructor,
            IMethodSymbol => MemberEntryKind.Method,
            IPropertySymbol => MemberEntryKind.Property,
            IFieldSymbol => MemberEntryKind.Field,
            IEventSymbol => MemberEntryKind.Event,
            _ => MemberEntryKind.Method
        };
    }
}