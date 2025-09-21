using Newtonsoft.Json;

namespace SkillMap.Shared.Extensions;

public static class JsonExtensions
{
    public static T DeserializeOrDefault<T>(this string json, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(json))
        {
            return defaultValue;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException)
        {
            return defaultValue;
        }
    }

    public static string SerializeOrDefault<T>(this T obj)
    {
        if (obj == null)
        {
            return null;
        }

        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static T DeepCopy<T>(this T obj)
    {
        if (obj == null)
        {
            return default;
        }

        try
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
