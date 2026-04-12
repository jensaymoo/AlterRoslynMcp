using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoslynMcp.Infrastructure._Refactored;

public sealed class MemberEntry
{
    public string SymbolName { get; init; }
    public string Signature { get; init; }
    
    public MemberEntryKind Kind { get; init; }
    public SymbolAccessibility Accessibility { get; init; }
    
    public IEnumerable<SourceLocation>? Location { get; init; }
    public string? Summary { get; init; }
    
    public bool IsStatic { get; init; }
    public bool IsInherited { get; init; }

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
    }
    
    private static bool IsInheritedFrom(ISymbol member, INamedTypeSymbol sourceType)
        => member.ContainingType != null 
           && !SymbolEqualityComparer.Default.Equals(member.ContainingType, sourceType);
}
