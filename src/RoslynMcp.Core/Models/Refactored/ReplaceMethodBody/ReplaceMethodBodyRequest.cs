namespace RoslynMcp.Core.Models;

public sealed record ReplaceMethodBodyRequest(
    string TargetMethodSymbolId,
    string Body);