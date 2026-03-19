using FluentValidation;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItem;

internal sealed class DeleteLearningItemRequestValidator : AbstractValidator<DeleteLearningItemRequest>
{
    public DeleteLearningItemRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Learning item id is required");
 }
}
