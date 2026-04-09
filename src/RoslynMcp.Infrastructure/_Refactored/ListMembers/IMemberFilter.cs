namespace RoslynMcp.Infrastructure._Refactored;

public interface IMemberFilter
{
    bool Matches(MemberEntry entry);
}
