using System;

using FluentValidation;

using SkillMap.Shared.Results;

using UserRole = SkillMap.Core.Constants.Role;

namespace SkillMap.Business.Account.Models;

public class UserRegistrationDto : AbstractValidator<UserRegistrationDto>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public UserRegistrationDto()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("User name is required.")
            .WithErrorCode(ErrorCode.USERNAMEREQUIRED);

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

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .WithErrorCode(ErrorCode.ROLEREQUIRED)
            .Must(r => UserRole.AVAILABLE_ROLES.Contains(r))
            .WithMessage("Role must be one of the available roles.")
            .WithErrorCode(ErrorCode.ROLEINVALID);
    }
}