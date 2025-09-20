namespace SkillMap.Core.Constants;

public static class Role
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Mentor = "Mentor";

    public static readonly string[] AVAILABLE_ROLES = new[] { Admin, User, Mentor };
}