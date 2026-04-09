using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface IMembersInheritanceCollector
{
    ImmutableArray<ISymbol> CollectWithInheritance(INamedTypeSymbol type);
}
