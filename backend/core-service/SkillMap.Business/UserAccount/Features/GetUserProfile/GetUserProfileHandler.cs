using MediatR;

using SkillMap.Api.UserAccount;
using SkillMap.Business.Abstractions;
using SkillMap.Business.Helpers;
using SkillMap.Core.User;

namespace SkillMap.Business.UserAccount.Features.GetUserProfile;

public class GetUserProfileHandler(IRepository<AppUser> userRepository, IProfileImagesService profileImageService)
    : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken) ?? throw new KeyNotFoundException($"User with id {request.UserId} not found");
        var imageAbsoluteUrl = await profileImageService.GetImageAbsoluteUriSafeAsync(user.ImageUrl, cancellationToken);
        return new UserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            ImageUrl = imageAbsoluteUrl,
            Role = user.Role
        };
    }
}
