using SkillMap.Application.Algo;
using SkillMap.Application.Domain;
using SkillMap.Application.Helpers;
using SkillMap.Application.InPorts.Retriever;
using SkillMap.Application.OutPorts.Persistence;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Application;

public class RoadmapRetriever : IRetriever
{
    private IRoadmapRepository RoadmapRepository { get; }
    public RoadmapRetriever(IRoadmapRepository roadmapRepository)
    {
        RoadmapRepository = roadmapRepository ?? throw new ArgumentNullException(nameof(roadmapRepository));
    }

    public async Task<Result<(List<NodeDto> Nodes, List<EdgeDto<NodeDto>> Edges)>> RetrieveByIdAsync(string roadmapId, CancellationToken ct = default)
    {
        var roadmapResult = await RoadmapRepository.GetRoadmap(roadmapId, ct);
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

    public async Task<Result<List<NodeDto>>> GetAllRoadmaps(CancellationToken ct = default)
    {
        return await RoadmapRepository.GetAllRoadmaps(ct);
    }
}
