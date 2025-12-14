using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Core;

public sealed class Topic
{
    public string Id { get; }
    public string Name { get; }
    public string? Description { get; }

    public Topic(string id, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Topic id is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Topic name is required");

        Id = id;
        Name = name;
        Description = description;
    }
}
