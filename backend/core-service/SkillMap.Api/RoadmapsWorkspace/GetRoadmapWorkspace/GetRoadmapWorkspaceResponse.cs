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
            LearningItems = dto.LearningItems.Select(li => new PersonalizedLearningItemResponse(li.Id, li.Title, li.Description, li.Status.ToStatusString(), li.Type)).ToList(),
            LearningItemsConnections = dto.LearningItemsConnections.Select(conn => new PersonalizedLearningItemsConnectionResponse(conn.Id, conn.FromId, conn.ToId)).ToList()
        };
    }
}

public class PersonalizedLearningItemResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    public PersonalizedLearningItemResponse(string id, string title, string description, string status, string type)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Type = type;
    }
}

public class PersonalizedLearningItemsConnectionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("source")]
    public string FromId { get; set; }
    [JsonPropertyName("target")]
    public string ToId { get; set; }
    public PersonalizedLearningItemsConnectionResponse(string id, string fromId, string toId)
    {
        Id = id;
        FromId = fromId;
        ToId = toId;
    }
}