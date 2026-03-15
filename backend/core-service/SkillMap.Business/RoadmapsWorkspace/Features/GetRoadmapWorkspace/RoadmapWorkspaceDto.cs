using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public class RoadmapWorkspaceDto
{
    public string Id { get; set; }
    public int Version { get; set; }

    public List<PersonalizedLearningItemDto> LearningItems { get; set; }
    public List<PersonalizedLearningItemsConnectionDto> LearningItemsConnections { get; set; }

    public static RoadmapWorkspaceDto Create(RoadmapSnapshot roadmapSnapshot)
    {
        return new RoadmapWorkspaceDto
        {
            Id = roadmapSnapshot.Id,
            LearningItems = roadmapSnapshot.LearningItems.Select(li => new PersonalizedLearningItemDto(li.Id, li.Title, li.Description, li.Status)).ToList(),
            LearningItemsConnections = roadmapSnapshot.LearningItemsConnections.Select(conn => new PersonalizedLearningItemsConnectionDto(conn.Id, conn.FromId, conn.ToId)).ToList()
        };
    }
}

public record PersonalizedLearningItemDto(string Id, string Title, string Description, LearningStatus Status);
public record PersonalizedLearningItemsConnectionDto(string Id, string FromId, string ToId);
