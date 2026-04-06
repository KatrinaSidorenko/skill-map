using FluentValidation;

using SkillMap.Core.Constants;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.UpdateLearningItem;

internal sealed class UpdateLearningItemRequestValidator : AbstractValidator<UpdateLearningItemRequest>
{
    public UpdateLearningItemRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Learning item id is required");
        RuleFor(r => r.Status)
            .Must(s => s is null || LearningStatusExtensions.GetStatuses().Contains(s))
            .WithMessage("Status must be one of the following: notstarted, inprogress, completed, skipped");
    }
}