using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public class RoadmapWorkspaceDto
{
    // todo: what is it? It roamdpId or workspace???
    public string Id { get; set; }
    public int Version { get; set; }
    public string Title { get; set; } // doubts about it

    public List<PersonalizedLearningItemDto> LearningItems { get; set; }
    public List<PersonalizedLearningItemsConnectionDto> LearningItemsConnections { get; set; }

    public static RoadmapWorkspaceDto Create(long workspaceId, RoadmapSnapshot roadmapSnapshot, int version, string title)
    {
        return new RoadmapWorkspaceDto
        {
            Id = workspaceId.ToString(),
            Version = version,
            Title = title,
            LearningItems = roadmapSnapshot?.LearningItems?.Select(li => new PersonalizedLearningItemDto(li.Id, li.Title, li.Description, li.Status)).ToList() ?? [],
            LearningItemsConnections = roadmapSnapshot?.LearningItemsConnections?.Select(conn => new PersonalizedLearningItemsConnectionDto(conn.Id, conn.FromId, conn.ToId)).ToList() ?? []
        };
    }
}

public record PersonalizedLearningItemDto(string Id, string Title, string Description, LearningStatus Status);
public record PersonalizedLearningItemsConnectionDto(string Id, string FromId, string ToId);
