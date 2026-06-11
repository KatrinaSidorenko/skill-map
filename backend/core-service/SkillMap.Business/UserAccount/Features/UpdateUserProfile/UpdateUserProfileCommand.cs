using MediatR;

using SkillMap.Business.Abstractions.Files;
using SkillMap.Shared.Files;

namespace SkillMap.Business.UserAccount.Features.UpdateUserProfile;

public record UpdateUserProfileCommand(long UserId, string? UserName, string? Email, HardFile? HardFile) : IRequest;
