namespace SkillMap.Api;

internal record Module(string Value)
{
    internal static readonly Module RoadmapsWorkspace = new("RoadmapsWorkspace");
    internal static readonly Module PersonalRoadmaps = new("PersonalRoadmaps");

    public static implicit operator string(Module module) => module.Value;
}
