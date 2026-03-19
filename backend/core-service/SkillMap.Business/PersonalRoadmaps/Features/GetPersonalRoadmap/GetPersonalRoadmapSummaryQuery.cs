namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;

public record GetPersonalRoadmapSummaryQuery(string PersonalRoadmapId, long UserId) : ICommand<PersonalRoadmapSummaryDto>
{
    public long GetPersonalRoadmapId() => long.Parse(PersonalRoadmapId);
}
