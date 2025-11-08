namespace SkillMap.Shared.Results;

public class LearningPlatformException : Exception
{
    public string Code { get; }
    public LearningPlatformException(string code, string message) : base(message)
    {
        Code = code;
    }

    public LearningPlatformException(string code): base(string.Empty)
    {
        Code = code;
    }
}
