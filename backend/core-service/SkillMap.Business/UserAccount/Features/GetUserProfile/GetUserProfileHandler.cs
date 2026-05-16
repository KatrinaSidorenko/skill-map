using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.User;

namespace SkillMap.Business.UserAccount.Features.GetUserProfile;

public class GetUserProfileHandler(IRepository<AppUser> userRepository)
    : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken) ?? throw new KeyNotFoundException($"User with id {request.UserId} not found");

        return new UserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            ImageUrl = user.ImageUrl,
            Role = user.Role
        };
    }
}
