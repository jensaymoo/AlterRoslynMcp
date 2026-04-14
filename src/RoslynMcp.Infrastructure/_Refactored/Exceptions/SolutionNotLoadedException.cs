namespace RoslynMcp.Infrastructure._Refactored;

public class SolutionNotLoadedException : Exception
{
    public SolutionNotLoadedException()
        : base("Solution not loaded") { }
}
