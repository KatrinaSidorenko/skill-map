namespace SkillMap.Core.RoadmapsWorkspace;

public enum MaterialType
{
    Article,
    Video,
    Book,
    Course,
    Other
}

public static class MaterialTypeExtensions
{
    public static MaterialType Parse(string? value) => value?.ToLowerInvariant() switch
    {
        "article" => MaterialType.Article,
        "video"   => MaterialType.Video,
        "book"    => MaterialType.Book,
        "course"  => MaterialType.Course,
         _        => MaterialType.Other
    };

    public static string ToTypeString(this MaterialType type) => type switch
    {
        MaterialType.Article => "article",
        MaterialType.Video   => "video",
        MaterialType.Book    => "book",
        MaterialType.Course  => "course",
         _                   => "other"
    };
}
