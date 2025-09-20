namespace SkillMap.Shared.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return default(TValue);
    }
}
