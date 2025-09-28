//using Microsoft.AspNetCore.Mvc;
//using SkillMap.Api.Models;
//using SkillMap.Api.Mappers;
//using SkillMap.Application;
//using SkillMap.Application.Domain;
//using SkillMap.Shared.Extensions;
//using SkillMap.Shared;
//using SkillMap.Application.InPorts.Migrator;
//using SkillMap.Application.InPorts.Roadmap;

//namespace SkillMap.Api.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class RoadmapsController : ControllerBase
//{
//    private IRetriever Retriever { get; }
//    public RoadmapsController(IRetriever retriever)
//    {
//        Retriever = retriever ?? throw new ArgumentNullException(nameof(retriever));
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAll(CancellationToken ct)
//    {
//        var roadmaps = await Retriever.GetAllRoadmaps(ct);
//        if (!roadmaps.IsSuccessful)
//        {
//            // todo: change 500
//            return BadRequest(roadmaps.Message);
//        }

//        var response = new RoadmapsResponse
//        {
//            Roadmaps = roadmaps.Data.ToRoadmaps().OrderBy(r => r.Title).ToList(),
//        };

//        return Ok(response);
//    }

//    [HttpGet("{roadmapId}/graph")]
//    public async Task<IActionResult> Get([FromRoute]string roadmapId, CancellationToken ct)
//    {
//        var result = await Retriever.RetrieveByIdAsync(roadmapId, ct);
//        if (!result.IsSuccessful)
//        {
//            return BadRequest(result.Message);
//        }

//        var (nodes, edges) = result.Data;
//        if (!nodes.Any() || !edges.Any())
//        {
//            return NotFound("No nodes or edges found for the specified roadmap.");
//        }

//        var graph = new Graph(nodes, edges);
//        var adjacencyList = graph.BuildAdjacencyList();
//        var rootNode = edges
//            .Select(e => e.Source)
//            .FirstOrDefault();

//        var sortedNodes = TopologicalSort.SortNodes(nodes, edges);
//        var ids = sortedNodes.Select(n => n.Id).ToList();
//        var sortedNodesWithIndex = sortedNodes
//            .Where(n => n.Type.IsTopic())
//            .Select((node, index) => new NodeResponse
//            {
//                Id = node.Id,
//                Title = node.Title.FirstCharToUpper(),
//                Index = index,
//                Description = node.Description,
//                Type = node.Type,
//                ParentId = rootNode.Id,
//                AdditionalProps = node.AdditionalProps,
//                Children = adjacencyList.GetOrDefault(node)?
//                    .Where(n => n.Type.IsSubTopic())
//                    .BuildNodes(adjacencyList, ids) ?? [],
//            })
//            .ToList();


//        var treeResponse = new TreeResponse
//        {
//            Id = rootNode.Id,
//            Title = rootNode.Title.FirstCharToUpper(),
//            Nodes = sortedNodesWithIndex,
//        };
//        return Ok(treeResponse);
//    }

//    [HttpGet("{roadmapId}")]
//    public async Task<IActionResult> GetPlainGraph([FromRoute] string roadmapId, CancellationToken ct)
//    {
//        var result = await Retriever.RetrieveByIdAsync(roadmapId, ct);
//        if (!result.IsSuccessful)
//        {
//            return BadRequest(result.Message);
//        }

//        var (nodes, edges) = result.Data;
//        var rootNode = nodes.Where(n => n.Type.IsRoadmap()).FirstOrDefault();
//        rootNode.Title = rootNode.Title.FirstCharToUpper();

//        var sortedNodes = TopologicalSort.SortNodes(nodes, edges);
//        var ids = sortedNodes.Select(n => n.Id).ToList();
        
//        var response = new TreePlainResponse
//        {
//            Nodes = sortedNodes.Select(node => node.ToPlainNode(ids)).ToList(),
//            Edges = edges.Select(e => e.ToPlainEdge()).ToList(),
//        };
//        return Ok(response);
//    }
//}
