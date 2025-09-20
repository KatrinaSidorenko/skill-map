using FluentValidation;
using SkillMap.Shared.Results;
using System;
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
            .WithErrorCode(ErrorCode.USERNAME_REQUIRED);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email is required.")
            .WithErrorCode(ErrorCode.EMAIL_INVALID);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .WithErrorCode(ErrorCode.PASSWORD_INVALID);

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .WithErrorCode(ErrorCode.ROLE_REQUIRED)
            .Must(r => UserRole.AVAILABLE_ROLES.Contains(r))
            .WithMessage("Role must be one of the available roles.")
            .WithErrorCode(ErrorCode.ROLE_INVALID);
    }
}
