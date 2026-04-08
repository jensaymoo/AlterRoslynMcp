namespace RoslynMcp.Infrastructure._Refactored.Exceptions;

public class LoadSolutionException: Exception
{
    public LoadSolutionException() : base()
    {
    }

    public LoadSolutionException(string? message) : base(message)
    {
    }

    public LoadSolutionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}