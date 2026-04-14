namespace RoslynMcp.Infrastructure._Refactored.Exceptions;

public class LoadSolutionException : Exception
{
    public LoadSolutionException(Exception innerException)
        : base(innerException.Message, innerException) { }
}
