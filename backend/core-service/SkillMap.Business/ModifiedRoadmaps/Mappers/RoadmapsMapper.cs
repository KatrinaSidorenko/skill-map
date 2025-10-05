using LearningPlatform.Roadmap.Business.Contracts.Constants;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Core.Constants;

namespace SkillMap.Business.Roadmaps.Mappers;

public static class RoadmapsMapper
{
    //public static CustomizedUserRoadmapLearningItem Map(this NodeResponse nodeResponse, string parentId)
    //{
    //    return new CustomizedUserRoadmapLearningItem
    //    {
    //        Id = nodeResponse.Id,
    //        Title = nodeResponse.Title,
    //        Description = nodeResponse.Description,
    //        Type = nodeResponse.Type,
    //        Status = nodeResponse.Status ?? LearningStatus.NotStarted.ToString(),
    //        Progress = nodeResponse.Progress ?? 0.0,
    //        ParentId = parentId,
    //        Index = nodeResponse.Index,
    //        AdditinalProps = nodeResponse.AdditionalProps?.ToDictionary(k => k.Key, v => (object)v.Value),
    //        Children = nodeResponse.Children?
    //            .Where(ch => !ch.Type.IsTopic())
    //            .OrderBy(x => x.Index)
    //            .Select(x => x.Map(nodeResponse.Id)).ToList()
    //    };
    //}

    public static NodeResponse Map(this PlainNodeResponse nodeResponse)
    {
        return new NodeResponse
        {
            Id = nodeResponse.Id,
            Title = nodeResponse.Title,
            Description = nodeResponse.Description,
            Type = nodeResponse.Type,
            Status = LearningStatus.NotStarted.ToString(),
            Progress = 0.0,
            ParentId = nodeResponse.ParentId,
            Index = nodeResponse.Index,
            AdditionalProps = nodeResponse.AdditionalProps,
            Children = new List<NodeResponse>()
        };
    }
}
