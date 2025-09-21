using SkillMap.Application.InPorts.Migrator;
using SkillMap.Application.OutPorts.DataSource;
using SkillMap.Application.OutPorts.Persistence;
using SkillMap.Shared;
using SkillMap.Application.Helpers;
using SkillMap.Application.Domain;
using SkillMap.Application.Algo;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;
using SkillMap.Application.Mappers;

namespace SkillMap.Application;

public class RoadmapMigrator : IMigrator
{
    private IRoadmapDataSource RoadmapDataSource { get; }
    private IRoadmapRepository RoadmapRepository { get; }

    public RoadmapMigrator(IRoadmapDataSource roadmapDataSource, IRoadmapRepository roadmapRepository)
    {
        RoadmapDataSource = roadmapDataSource ?? throw new ArgumentNullException(nameof(roadmapDataSource));
        RoadmapRepository = roadmapRepository ?? throw new ArgumentNullException(nameof(roadmapRepository));
    }

    public async Task<bool> MigrateAsync(DataSourceConfig config, CancellationToken ct = default)
    {
        var graph = await RoadmapDataSource.GetRoadmapSource(config, ct);
        var nodes = graph.Nodes;
        var edges = graph.Edges;

        var validNodes = nodes
            .Where(x => x.IsValid())
            .DistinctBy(n => n.ExternalId)
            .ToList();

        var validEdges = edges
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

        if (!validNodes.HasRoadmapNode())
        {
            var rootNode = validEdges.GetRootNode();
            if (rootNode == null)
            {
                throw new ArgumentNullException("Root node not found");
            }

            var roadmapNode = new NodeDto
            {
                ExternalId = config.RoadmapName,
                Title = config.RoadmapName,
                Type = NodeType.Roadmap
            };
            validNodes.Add(roadmapNode);

            validEdges.Add(new EdgeDto<NodeDto>
            {
                Source = roadmapNode,
                Target = rootNode,
            });
        }

        return await RoadmapRepository.Save((validNodes, validEdges), ct);
    }

    public async Task<Result<bool>> MigrateNodesDescriptions(DataSourceConfig config, CancellationToken ct)
    {
        var files = await RoadmapDataSource.GetFolderContent(config, ct);
        if (files == null || files.Count == 0)
        {
            return Result.Failure<bool>("FAIL_TO_MIGRATE_NODES", "fail");
        }

        var parsedContents = new Dictionary<string, (string, List<ResourceDto>)>();
        foreach(var file in files)
        {
            var content = await RoadmapDataSource.ParseFileContent(file, ct);
            if (content.Description == null && content.ResourceDtos == null)
            {
                var yy = 0;
                continue;
            }
            parsedContents.Add(file.Name, content);
        }

        var result = await RoadmapRepository.GetRoadmap(config.RoadmapId, ct);
        if (!result.IsSuccessful)
        {
            return Result.Failure<bool>("FAIL_TO_GET_ROADMAP", "fail");
        }

        var (nodes, edges) = result.Data;

        var nodesResourcesDict = nodes
            .ToDictionary(n => n.ExternalId, n => parsedContents.Where(p => p.Key.Contains(n.ExternalId)).Select(p => (p.Value.Item1, p.Value.Item2?.Select(r => r.ToNode()).ToList())).FirstOrDefault());

        var nodeNewDescriptionDict = nodesResourcesDict
            .Where(n => n.Value.Item1 != null)
            .ToDictionary(n => n.Key, n => n.Value.Item1);

        var descriptionUpdateResult = await RoadmapRepository.UpdateNodesDescription(nodeNewDescriptionDict, ct);
        if (!descriptionUpdateResult.IsSuccessful)
        {
            return Result.Failure<bool>("FAIL_TO_UPDATE_NODES_DESCRIPTION", "fail");
        }

        var nodesWithResources = nodesResourcesDict
            .Where(n => n.Value.Item2 != null)
            .ToDictionary(n => n.Key, n => n.Value.Item2.Select(r => r).ToList());

        // migrate nodes of resources
        var nodesToMigrate = nodesWithResources
            .SelectMany(n => n.Value)
            .Distinct()
            .ToList();

        var allEdges = nodes
            .Where(n => nodesResourcesDict.ContainsKey(n.ExternalId))
            .SelectMany(n => nodesResourcesDict[n.ExternalId].Item2?.Select(r => new EdgeDto<NodeDto>
            {
                Source = n,
                Target = r,
            }) ?? []).ToList();

        var migrationResult = await RoadmapRepository.Save((nodesToMigrate, allEdges), ct);

        return Result.Success<bool>(migrationResult);
    }
}
