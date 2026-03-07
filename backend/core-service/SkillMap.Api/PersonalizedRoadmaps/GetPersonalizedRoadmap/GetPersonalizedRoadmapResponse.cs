using SkillMap.Business.PersonalizedRoadmaps.GetPersonalizedRoadmap;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;

public class GetPersonalizedRoadmapResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public double Progress { get; set; }
    public DateTime SavedAt { get; set; }
    public string Status { get; set; }

    public List<PersonalizedLearningItemResponse> LearningItems { get; set; }
    public List<PersonalizedLearningItemsConnectionResponse> LearningItemsConnections { get; set; }

    public static GetPersonalizedRoadmapResponse Create(PersonalizedRoadmapDto dto)
    {
        return new GetPersonalizedRoadmapResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Progress = dto.Progress,
            SavedAt = dto.SavedAt,
            Status = dto.Status,
            LearningItems = dto.LearningItems.Select(li => new PersonalizedLearningItemResponse(li.Id, li.Title, li.Description, li.Status.ToStatusString())).ToList(),
            LearningItemsConnections = dto.LearningItemsConnections.Select(conn => new PersonalizedLearningItemsConnectionResponse(conn.Id, conn.FromId, conn.ToId)).ToList()
        };
    }
}

public record PersonalizedLearningItemResponse(string Id, string Title, string Description, string Status);
public record PersonalizedLearningItemsConnectionResponse(string Id, string FromId, string ToId);


