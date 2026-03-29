namespace RoslynMcp.Core.Models;

public sealed record LoadSolutionRequest(string? SolutionHintPath = null);