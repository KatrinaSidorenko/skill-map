namespace SkillMap.Api;

internal record Module(string Value)
{
    internal static readonly Module PersonalizedRoadmaps = new("PersonalizedRoadmaps");

    public static implicit operator string(Module module) => module.Value;
}
