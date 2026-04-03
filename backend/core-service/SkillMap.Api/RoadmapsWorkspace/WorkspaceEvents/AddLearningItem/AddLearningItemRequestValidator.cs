using FluentValidation;

using SkillMap.Core.Constants;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.AddLearningItem;

internal sealed class AddLearningItemRequestValidator : AbstractValidator<AddLearningItemRequest>
{
    public AddLearningItemRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(r => r.Status).NotEmpty().WithMessage("Status is required")
            .Must(s => LearningStatusExtensions.GetStatuses().Contains(s))
            .WithMessage("Status must be one of the following: Planned, InProgress, Completed");
        RuleFor(r => r.Type).NotEmpty().WithMessage("Type is required")
            .Must(t => LearningItemTypeExtensions.GetTypes().Contains(t))
            .WithMessage("Type must be one of the following: Skill, Project, Course, Article, Video, Book");
    }
}
