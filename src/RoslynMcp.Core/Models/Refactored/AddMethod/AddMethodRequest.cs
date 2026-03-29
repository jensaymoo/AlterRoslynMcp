namespace RoslynMcp.Core.Models;

public sealed record AddMethodRequest(
    string TargetTypeSymbolId,
    MethodInsertionSpec Method);