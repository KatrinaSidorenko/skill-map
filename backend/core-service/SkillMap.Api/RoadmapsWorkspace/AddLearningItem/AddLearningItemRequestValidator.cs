using FluentValidation;

using SkillMap.Core.Constants;

namespace SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

internal sealed class AddLearningItemRequestValidator : AbstractValidator<AddLearningItemRequest>
{
    public AddLearningItemRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(r => r.Status).NotEmpty().WithMessage("Status is required")
            .Must(s => LearningStatusExtensions.GetStatuses().Contains(s))
            .WithMessage("Status must be one of the following: Planned, InProgress, Completed");
    }
}
