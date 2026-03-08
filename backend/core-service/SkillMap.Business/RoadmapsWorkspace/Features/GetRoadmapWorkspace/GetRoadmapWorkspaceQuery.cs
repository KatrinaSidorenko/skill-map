namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public record GetRoadmapWorkspaceQuery(long UserRoadmapId) : ICommand<RoadmapWorkspaceDto>;
