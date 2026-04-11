using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

internal static class SourceLocationExtension
{
    internal static SourceLocation? AsSourceLocation(this Location location)
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

    internal static IEnumerable<SourceLocation> AsSourceLocations(this IEnumerable<Location> locations)
        => locations
            .Select(l => l.AsSourceLocation())
            .OfType<SourceLocation>();
}