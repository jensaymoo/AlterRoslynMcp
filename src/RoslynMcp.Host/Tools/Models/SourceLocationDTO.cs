namespace RoslynMcp.Host.Tools.Models;

public record SourceLocationDTO (
    string FilePath,
    int Line,
    int Column
);