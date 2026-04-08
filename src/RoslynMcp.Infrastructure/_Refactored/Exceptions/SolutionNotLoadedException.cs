namespace RoslynMcp.Infrastructure._Refactored;

public class SolutionNotLoadedException: Exception
{
    public SolutionNotLoadedException() : base()
    {
    }

    public SolutionNotLoadedException(string? message) : base(message)
    {
    }

    public SolutionNotLoadedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}