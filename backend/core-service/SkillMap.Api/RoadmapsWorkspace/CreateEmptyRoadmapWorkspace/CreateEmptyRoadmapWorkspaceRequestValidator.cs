using FluentValidation;

namespace SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;

public class CreateEmptyRoadmapWorkspaceRequestValidator : AbstractValidator<CreateEmptyRoadmapWorkspaceRequest>
{
    public CreateEmptyRoadmapWorkspaceRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(r => r.Title).MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        RuleFor(r => r.Description).MaximumLength(1000).When(r => r.Description is not null).WithMessage("Description must not exceed 1000 characters");
    }
}
