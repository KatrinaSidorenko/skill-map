namespace SkillMap.Shared.Results;

public record ExceptionResult(string Code, string Message);
public class LearningPlatformException : Exception
{
    public string Code { get; }

    public LearningPlatformException(ExceptionResult result) : base(result.Message)
    {
        Code = result.Code;
    }
    public LearningPlatformException(string code, string message) : base(message)
    {
        Code = code;
    }

    public LearningPlatformException(string code): base(string.Empty)
    {
        Code = code;
    }
}
