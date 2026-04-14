namespace RoslynMcp.Infrastructure._Refactored;

public class SymbolEntryNotFoundException : Exception
{
    public SymbolEntryNotFoundException(string symbolId)
        : base($"Symbol '{symbolId}' not found in solution") { }

    public SymbolEntryNotFoundException(string symbolId, string projectName)
        : base($"Symbol '{symbolId}' not found in project '{projectName}'") { }
}
