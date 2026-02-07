using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Core;

public sealed class Topic
{
    public long Id { get; set; }
    public string ExternalId { get; }
    public string Name { get; }
    public string? Description { get; }

    public Topic(string externalId, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new LearningPlatformException(ErrorCode.VALIDATIONERROR, "Topic id is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new LearningPlatformException(ErrorCode.VALIDATIONERROR, "Topic name is required");

        ExternalId = externalId;
        Name = name;
        Description = description;
    }
}