namespace RoslynMcp.Infrastructure._Refactored;

public class MemberFilter(MemberEntryKind? kind, SymbolAccessibility? accessibility, bool? isStatic) : IMemberFilter
{
    public bool Matches(MemberEntry entry)
    {
        if (kind != null && entry.Kind != kind)
            return false;

        if (accessibility != null && entry.Accessibility != accessibility)
            return false;

        if (isStatic != null && entry.IsStatic != isStatic)
            return false;

        return true;
    }
}
