namespace SkillMap.Shared.Results;

public class ErrorCode
{
    // Prefixes
    public const string SYSTEM_ERROR_PREFIX = "SE";
    public const string USER_INPUT_ERROR_PREFIX = "UIE";

    // System Errors
    public const string FAILED_TO_SAVE      = SYSTEM_ERROR_PREFIX + "1"; // SE1
    public const string FAILED_TO_CREATE    = SYSTEM_ERROR_PREFIX + "2"; // SE2
    public const string FAILED_TO_DELETE    = SYSTEM_ERROR_PREFIX + "3"; // SE3
    public const string FAILED_TO_UPDATE    = SYSTEM_ERROR_PREFIX + "4"; // SE4
    public const string FAILED_TO_GET       = SYSTEM_ERROR_PREFIX + "5"; // SE5
    public const string NOT_FOUND           = SYSTEM_ERROR_PREFIX + "6"; // SE6
    public const string INTERNAL_ERROR      = SYSTEM_ERROR_PREFIX + "7"; // SE7
    public const string TIMEOUT             = SYSTEM_ERROR_PREFIX + "8"; // SE8
    public const string UNAUTHORIZED        = SYSTEM_ERROR_PREFIX + "9"; // SE9
    public const string FORBIDDEN           = SYSTEM_ERROR_PREFIX + "10"; // SE10
    public const string CONFLICT            = SYSTEM_ERROR_PREFIX + "11"; // SE11

    // User Input Errors
    public const string VALIDATION_ERROR    = USER_INPUT_ERROR_PREFIX + "0"; // UIE0
    public const string INVALID_PASSWORD    = USER_INPUT_ERROR_PREFIX + "1"; // UIE1
    public const string USER_ALREADY_EXISTS = USER_INPUT_ERROR_PREFIX + "2"; // UIE2
    public const string USERNAME_REQUIRED   = USER_INPUT_ERROR_PREFIX + "3"; // UIE3
    public const string EMAIL_INVALID       = USER_INPUT_ERROR_PREFIX + "4"; // UIE4
    public const string PASSWORD_INVALID    = USER_INPUT_ERROR_PREFIX + "5"; // UIE5
    public const string ROLE_REQUIRED       = USER_INPUT_ERROR_PREFIX + "6"; // UIE6
    public const string ROLE_INVALID        = USER_INPUT_ERROR_PREFIX + "7"; // UIE7
    public const string EMAIL_REQUIRED      = USER_INPUT_ERROR_PREFIX + "8"; // UIE8
    public const string PASSWORD_REQUIRED   = USER_INPUT_ERROR_PREFIX + "9"; // UIE9
    public const string INVALID_INPUT       = USER_INPUT_ERROR_PREFIX + "10"; // UIE10
}
