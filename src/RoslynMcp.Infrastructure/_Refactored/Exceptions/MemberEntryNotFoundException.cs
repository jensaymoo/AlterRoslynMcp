namespace RoslynMcp.Infrastructure._Refactored;

public class MemberEntryNotFoundException : Exception
{
    public MemberEntryNotFoundException() : base()
    {
    }

    public MemberEntryNotFoundException(string? message) : base(message)
    {
    }

    public MemberEntryNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
