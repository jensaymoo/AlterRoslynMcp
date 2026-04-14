namespace RoslynMcp.Infrastructure._Refactored;

public class MemberEntryNotFoundException : Exception
{
    public MemberEntryNotFoundException(string symbolId)
        : base($"Member '{symbolId}' not found in solution") { }

    public MemberEntryNotFoundException(string symbolId, string projectName)
        : base($"Member '{symbolId}' not found in project '{projectName}'") { }
}
