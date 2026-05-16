using MediatR;

namespace SkillMap.Business.UserAccount.Features.GetUserProfile;

public record GetUserProfileQuery(long UserId) : IRequest<UserProfileDto>;
