namespace RoslynMcp.Infrastructure._Refactored;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(string projectName)
        : base($"Project '{projectName}' not found") { }
}
