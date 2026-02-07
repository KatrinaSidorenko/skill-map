using FluentValidation;

using SkillMap.Shared.Results;

namespace SkillMap.Business.Account.Models;

public class LoginDto : AbstractValidator<LoginDto>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public LoginDto()
    {

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email is required.")
            .WithErrorCode(ErrorCode.EMAILINVALID);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .WithErrorCode(ErrorCode.PASSWORDINVALID);
    }
}