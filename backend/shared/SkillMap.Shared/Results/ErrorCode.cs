namespace SkillMap.Shared.Results;

public class ErrorCode
{
    public const string FAILED_TO_SAVE = "SE1";
    public const string FAILED_TO_CREATE = "SE2";
    public const string FAILED_TO_DELETE = "SE3";
    public const string FAILED_TO_UPDATE = "SE4";
    public const string FAILED_TO_GET = "SE5";
    public const string NOT_FOUND = "SE6";


    public const string VALIDATION_ERROR = "UIE0";

    public const string INVALID_PASSWORD = "UIE1";
    public const string USER_ALREADY_EXISTS = "UIE2";
    public const string USERNAME_REQUIRED = "UIE3";
    public const string EMAIL_INVALID = "UIE4";
    public const string PASSWORD_INVALID = "UIE5";
    public const string ROLE_REQUIRED = "UIE6";
    public const string ROLE_INVALID = "UIE7";
}
