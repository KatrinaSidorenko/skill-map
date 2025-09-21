namespace SkillMap.Shared.Extensions;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => input,
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };
}
