using System.Reflection;

using SkillMap.Api.UserAccount.GetUserProfile;
using SkillMap.Shared;

namespace SkillMap.Api.UserAccount;

public static class UserAccountModule
{
    private static Assembly CurrentModule => typeof(UserProfileResponse).Assembly;

    public static IServiceCollection AddUserAccount(this IServiceCollection services)
    {
        services.AddRequestsValidations(CurrentModule);

        var handlersAssembly = typeof(Business.UserAccount.Features.GetUserProfile.GetUserProfileHandler).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(handlersAssembly));

        return services;
    }

    public static void RegisterUserAccount(this WebApplication app)
    {
        app.MapUserAccount();
    }
}
