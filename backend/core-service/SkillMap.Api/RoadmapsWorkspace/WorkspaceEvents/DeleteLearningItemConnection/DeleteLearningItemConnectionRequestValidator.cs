using FluentValidation;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItemConnection;

internal sealed class DeleteLearningItemConnectionRequestValidator : AbstractValidator<DeleteLearningItemConnectionRequest>
{
    public DeleteLearningItemConnectionRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Connection id is required");
    }
}
