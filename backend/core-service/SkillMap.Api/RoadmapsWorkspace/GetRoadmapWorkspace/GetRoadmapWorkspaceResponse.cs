using System.Text.Json.Serialization;

using SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;

public class GetRoadmapWorkspaceResponse
{
    [JsonPropertyName("workspaceId")]
    public string Id { get; set; }

    [JsonPropertyName("items")]
    public List<PersonalizedLearningItemResponse> LearningItems { get; set; }
    [JsonPropertyName("connections")]
    public List<PersonalizedLearningItemsConnectionResponse> LearningItemsConnections { get; set; }

    public static GetRoadmapWorkspaceResponse Create(RoadmapWorkspaceDto dto)
    {
        return new GetRoadmapWorkspaceResponse
        {
            Id = dto.Id,
            LearningItems = dto.LearningItems.Select(li => new PersonalizedLearningItemResponse(li.Id, li.Title, li.Description, li.Status.ToStatusString())).ToList(),
            LearningItemsConnections = dto.LearningItemsConnections.Select(conn => new PersonalizedLearningItemsConnectionResponse(conn.Id, conn.FromId, conn.ToId)).ToList()
        };
    }
}

public record PersonalizedLearningItemResponse(string Id, string Title, string Description, string Status);
public record PersonalizedLearningItemsConnectionResponse(string Id, string FromId, string ToId);


