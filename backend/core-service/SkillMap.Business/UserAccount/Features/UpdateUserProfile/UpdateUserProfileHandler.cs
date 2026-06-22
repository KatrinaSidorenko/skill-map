using MediatR;

using SkillMap.Api.UserAccount;
using SkillMap.Business.Abstractions;
using SkillMap.Core.User;
using SkillMap.Shared.Results;

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
        {
            var userWithRequestedEmail = await userRepository.GetFirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
            if (userWithRequestedEmail is not null)
            {
                throw new ArgumentException($"User with email {request.Email} already exists");
            }
            user.Email = request.Email;
        }

        var prevUserProfileImageUrl = user.ImageUrl;
        if (request.HardFile is not null)
        {
            var uploadResult = await profileImageService.UploadImageAsync(request.HardFile, cancellationToken);
            user.ImageUrl = uploadResult.RelativePath;
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        if (request.HardFile is null) return;
        if (string.IsNullOrEmpty(prevUserProfileImageUrl)) return;
        var deleteResult = await profileImageService.DeleteImageAsync(prevUserProfileImageUrl, cancellationToken);
        if (!deleteResult)
        {
            // todo: Log the failure to delete the old image, but continue with the update
            // as the new image will still be uploaded and set for the user.
        }
    }
}
