using SkillMap.Shared.Models;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;
public record GetPersonalRoadmapsQuery(long UserId, FilteringParams FilteringParams) : ICommand<PaginationResult<PersonalRoadmapDto>>;