using SkillMap.Shared.Models;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;
public record GetPersonalRoadmapsQuery(long UserId, PaginationParams PaginationParams) : ICommand<PaginationResult<PersonalRoadmapDto>>;