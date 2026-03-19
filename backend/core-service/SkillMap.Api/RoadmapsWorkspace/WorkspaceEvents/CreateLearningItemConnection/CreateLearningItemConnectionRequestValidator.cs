using FluentValidation;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.CreateLearningItemConnection;

internal sealed class CreateLearningItemConnectionRequestValidator : AbstractValidator<CreateLearningItemConnectionRequest>
{
    public CreateLearningItemConnectionRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Connection id is required");
        RuleFor(r => r.Source).NotEmpty().WithMessage("Source learning item id is required");
        RuleFor(r => r.Target).NotEmpty().WithMessage("Target learning item id is required");
        RuleFor(r => r).Must(r => r.Source != r.Target).WithMessage("Source and target must be different learning items");
    }
}
