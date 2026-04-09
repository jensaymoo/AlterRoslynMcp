using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface IMemberExtractor
{
    string GetDisplayName(ISymbol member);
    string GetSignature(ISymbol member);
    MemberEntryKind GetKind(ISymbol member);
    bool GetIsStatic(ISymbol member);
}
