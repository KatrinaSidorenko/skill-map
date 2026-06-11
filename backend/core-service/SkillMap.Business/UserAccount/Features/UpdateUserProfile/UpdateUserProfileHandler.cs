using MediatR;

using SkillMap.Api.UserAccount;
using SkillMap.Business.Abstractions;
using SkillMap.Core.User;

namespace SkillMap.Business.UserAccount.Features.UpdateUserProfile;

public class UpdateUserProfileHandler(IRepository<AppUser> userRepository, IProfileImagesService profileImageService)
    : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
              ?? throw new KeyNotFoundException($"User with id {request.UserId} not found");

        if (request.UserName is not null)
            user.UserName = request.UserName;

        if (request.Email is not null)
            user.Email = request.Email;

        if (request.HardFile is not null)
        {
            // todo: the logic for previos image
            var uploadResult = await profileImageService.UploadImageAsync(request.HardFile, cancellationToken);
            user.ImageUrl = uploadResult.RelativePath;
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
