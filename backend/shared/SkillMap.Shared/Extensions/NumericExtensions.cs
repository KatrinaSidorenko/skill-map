namespace SkillMap.Shared.Extensions;

public static class NumericExtensions
{
    public static long? GetLongOrDefault(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (long.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }

    public static string ToStringWithoutHyphens(this Guid guid) => guid.ToString("N");
}
