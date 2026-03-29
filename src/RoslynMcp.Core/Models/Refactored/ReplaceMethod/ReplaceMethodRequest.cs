namespace RoslynMcp.Core.Models;

public sealed record ReplaceMethodRequest(
    string TargetMethodSymbolId,
    MethodInsertionSpec Method);