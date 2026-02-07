using LearningPlatform.Roadmap.Business.Algo;
using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Helpers;

using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business;

public class RoadmapRetriever : IRetriever
{
    private IRoadmapRepository RoadmapRepository { get; }
    public RoadmapRetriever(IRoadmapRepository roadmapRepository)
    {
        RoadmapRepository = roadmapRepository ?? throw new ArgumentNullException(nameof(roadmapRepository));
    }

    public async Task<Result<(List<NodeDto> Nodes, List<EdgeDto> Edges)>> RetrieveByIdAsync(string roadmapId, CancellationToken ct = default)
    {
        var roadmapResult = await RoadmapRepository.GetRoadmapById(roadmapId, ct);
        if (!roadmapResult.IsSuccessful)
        {
            return roadmapResult;
        }
        var validNodes = roadmapResult.Data.Nodes
            .Where(x => x.IsValid())
            .DistinctBy(n => n.ExternalId)
            .ToList();
        var validEdges = roadmapResult.Data.Edges
            .Where(x => x.IsValid())
            .Distinct()
            .ToList();
        var graph2 = new Graph(validNodes, validEdges);
        var sccComponents = new TarjanSccDetector(graph2).FindStronglyConnectedComponents();
        if (sccComponents.IsCyclic())
        {
            var sccs = GraphHelpers.GetSCCs(sccComponents);
            var cycleEdges = sccs.ResolveCycles(validEdges);
            validEdges = validEdges.Except(cycleEdges).ToList();
        }

        return Result.Success((validNodes, validEdges));
    }

    //public async Task<Result<List<NodeDto>>> GetAllRoadmaps(CancellationToken ct = default)
    //{
    //    return await RoadmapRepository.GetAllPlainRoadmaps(ct);
    //}
}