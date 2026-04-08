using SkillMap.Core.Roadmaps;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;
public record PersonalRoadmapsDto(List<PersonalRoadmapDto> PersonalRoadmaps)
{
    public static PersonalRoadmapsDto Create(IEnumerable<PersonalRoadmap> personalRoadmaps)
        => new PersonalRoadmapsDto(personalRoadmaps.Select(PersonalRoadmapDto.Create).ToList());
}
public record PersonalRoadmapDto(string Id, string Title, string Description, string ImageUrl, DateTime CreatedAt)
{
    public static PersonalRoadmapDto Create(PersonalRoadmap roadmap)
    {
        return new PersonalRoadmapDto(
            roadmap.Id.ToString(),
            roadmap.Title,
            roadmap.Description,
            roadmap.ImageUrl,
            roadmap.CreatedAt
        );
    }
}