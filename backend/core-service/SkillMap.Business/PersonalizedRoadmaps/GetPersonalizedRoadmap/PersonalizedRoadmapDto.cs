using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.PersonalizedRoadmaps.GetPersonalizedRoadmap;
public class PersonalizedRoadmapDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public double Progress { get; set; }
    public DateTime SavedAt { get; set; }
    public string Status { get; set; }

    public List<PersonalizedLearningItemDto> LearningItems { get; set; }
    public List<PersonalizedLearningItemsConnectionDto> LearningItemsConnections { get; set; }

    public static PersonalizedRoadmapDto Create(RoadmapSnapshot roadmapSnapshot)
    {
        return new PersonalizedRoadmapDto
        {
            Id = roadmapSnapshot.Id,
            Title = roadmapSnapshot.Title,
            Description = roadmapSnapshot.Description,
            ImageUrl = roadmapSnapshot.ImageUrl,
            Progress = roadmapSnapshot.Progress,
            SavedAt = roadmapSnapshot.SavedAt,
            Status = roadmapSnapshot.Status,
            LearningItems = roadmapSnapshot.LearningItems.Select(li => new PersonalizedLearningItemDto(li.Id, li.Title, li.Description, li.Status)).ToList(),
            LearningItemsConnections = roadmapSnapshot.LearningItemsConnections.Select(conn => new PersonalizedLearningItemsConnectionDto(conn.Id, conn.FromId, conn.ToId)).ToList()
        };
    }
}

public record PersonalizedLearningItemDto(string Id, string Title, string Description, LearningStatus Status);
public record PersonalizedLearningItemsConnectionDto(string Id, string FromId, string ToId);
