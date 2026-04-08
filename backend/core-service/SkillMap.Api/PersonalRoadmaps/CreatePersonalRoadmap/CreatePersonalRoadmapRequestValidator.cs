using FluentValidation;

namespace SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;

public class CreatePersonalRoadmapRequestValidator : AbstractValidator<CreatePersonalRoadmapRequest>
{
    public CreatePersonalRoadmapRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("Title is required");
    }
}