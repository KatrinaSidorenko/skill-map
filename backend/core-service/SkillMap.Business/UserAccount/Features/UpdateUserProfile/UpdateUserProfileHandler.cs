using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.User;

namespace SkillMap.Business.UserAccount.Features.UpdateUserProfile;

public class UpdateUserProfileHandler(IRepository<AppUser> userRepository)
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

        if (request.ImageUrl is not null)
            user.ImageUrl = request.ImageUrl;

        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
