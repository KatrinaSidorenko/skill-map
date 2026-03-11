namespace SkillMap.Api;

internal record Module(string Value)
{
    internal static readonly Module RoadmapsWorkspace = new("RoadmapsWorkspace");
    internal static readonly Module PersonalRoadmaps = new("PersonalRoadmaps");
    internal static readonly Module RoadmapBlueprints = new("RoadmapBlueprints");

    public static implicit operator string(Module module) => module.Value;
}
