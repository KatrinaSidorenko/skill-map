using MediatR;

namespace SkillMap.Business.UserAccount.Features.UpdateUserProfile;

public record UpdateUserProfileCommand(long UserId, string? UserName, string? Email, string? ImageUrl) : IRequest;
