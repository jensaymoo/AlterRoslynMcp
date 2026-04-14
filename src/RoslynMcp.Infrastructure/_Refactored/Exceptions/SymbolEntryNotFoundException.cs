namespace RoslynMcp.Infrastructure._Refactored;

public class SymbolEntryNotFoundException : Exception
{
    public SymbolEntryNotFoundException() : base() { }
    public SymbolEntryNotFoundException(string? message) : base(message) { }
    public SymbolEntryNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
