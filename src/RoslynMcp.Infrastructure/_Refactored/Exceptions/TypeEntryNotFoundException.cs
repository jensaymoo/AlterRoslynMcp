namespace RoslynMcp.Infrastructure._Refactored;

public class TypeEntryNotFoundException : Exception
{
    public TypeEntryNotFoundException(string symbolId)
        : base($"Type '{symbolId}' not found in solution") { }

    public TypeEntryNotFoundException(string symbolId, string projectName)
        : base($"Type '{symbolId}' not found in project '{projectName}'") { }
}
