namespace RoslynMcp.Core.Models;

public sealed record RunTestsRequest(string? Target = null, string? Filter = null);