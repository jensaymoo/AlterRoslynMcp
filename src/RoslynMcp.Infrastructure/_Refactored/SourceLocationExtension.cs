using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

internal static class SourceLocationExtension
{
    public static SourceLocation? AsSourceLocation(this Location location)
    {
        if (!location.IsInSource)
            return null;

        var span = location.GetLineSpan();
        var start = span.StartLinePosition;

        return new SourceLocation
        {
            FilePath = span.Path,
            Line = start.Line + 1,
            Column = start.Character + 1
        };
    }

    public static IEnumerable<SourceLocation?> AsSourceLocations(this IEnumerable<Location> locations)
    {
        return locations.Select(l => l.AsSourceLocation());
    }
}