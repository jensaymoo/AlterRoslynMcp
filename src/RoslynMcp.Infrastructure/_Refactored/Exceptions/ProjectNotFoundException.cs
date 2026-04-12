namespace RoslynMcp.Infrastructure._Refactored;

public class ProjectNotFoundException: Exception
{
    public ProjectNotFoundException() : base()
    {
    }

    public ProjectNotFoundException(string? message) : base(message)
    {
    }

    public ProjectNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}