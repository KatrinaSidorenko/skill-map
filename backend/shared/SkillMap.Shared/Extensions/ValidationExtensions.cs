using FluentValidation.Results;

namespace SkillMap.Shared.Extensions;

public static class ValidationExtensions
{
    public static List<string> GetErrors(this ValidationResult result)
        => result.Errors.Select(e => e.PropertyName + ". Message: " + e.ErrorMessage + "Code:" + e.ErrorCode).ToList();
}
