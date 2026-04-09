namespace RoslynMcp.Host.Tools.Models;

public record ListTypesResultDTO(
    ProjectSummaryDTO Project, 
    IEnumerable<TypeEntryDTO> Types);