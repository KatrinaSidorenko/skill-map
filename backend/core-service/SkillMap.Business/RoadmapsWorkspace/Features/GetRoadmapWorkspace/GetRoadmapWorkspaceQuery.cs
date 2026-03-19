namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public record GetRoadmapWorkspaceQuery(long WorkspaceId) : ICommand<RoadmapWorkspaceDto>;
