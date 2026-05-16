using SkillMap.Api.UserAccount.GetUserProfile;
using SkillMap.Api.UserAccount.UpdateUserProfile;

namespace SkillMap.Api.UserAccount;

internal static class UserAccountEndpoints
{
    internal static void MapUserAccount(this WebApplication app)
    {
        app.MapGetUserProfile();
        app.MapUpdateUserProfile();
    }
}
