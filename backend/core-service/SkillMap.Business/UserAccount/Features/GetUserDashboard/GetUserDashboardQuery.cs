using MediatR;

namespace SkillMap.Business.UserAccount.Features.GetUserDashboard;

public record GetUserDashboardQuery(long UserId) : IRequest<UserDashboardDto>;
