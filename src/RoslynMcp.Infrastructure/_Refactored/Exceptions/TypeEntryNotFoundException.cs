namespace RoslynMcp.Infrastructure._Refactored;

public class TypeEntryNotFoundException: Exception
{
    public TypeEntryNotFoundException() : base()
    {
    }

    public TypeEntryNotFoundException(string? message) : base(message)
    {
    }

    public TypeEntryNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}