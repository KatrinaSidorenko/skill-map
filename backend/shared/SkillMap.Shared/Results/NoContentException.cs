namespace SkillMap.Shared.Results;
public class NoContentException : Exception
{
    public NoContentException(string message)
        : base(message)
    {
    }

    public NoContentException()
        : base("No content available.")
    {
    }
}
