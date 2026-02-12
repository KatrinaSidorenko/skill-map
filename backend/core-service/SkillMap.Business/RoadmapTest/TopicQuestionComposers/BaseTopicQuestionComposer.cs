using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionDistributionBuilder;
using SkillMap.Business.RoadmapTest.TopicSelectors;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest.TopicQuestionComposers;

public record TopicTestBundle(List<TopicQuestionsDto> Questions, TopicQuestionsSettingDto CreationSettings);
public record RoadmapTestResult(Dictionary<string, TopicTestBundle> ByTopic);

internal sealed class NodeMeta
{
    public Node Node { get; }
    public int InDegree { get; set; }
    public int OutDegree { get; set; }
    public int Depth { get; set; }

    public HashSet<string> Ancestors { get; } = new();
    public HashSet<string> Descendants { get; } = new();

    public double StructuralInfluence { get; set; }

    public NodeMeta(Node node)
    {
        Node = node;
    }
}

internal sealed class Graph
{
    public IReadOnlyList<Node> Nodes { get; init; }
    public IReadOnlyList<Edge> Edges { get; init; }
}

public class BaseTopicQuestionComposer(
    ITopicQuestionsGenerator topicQuestionsGenerator,
    IRoadmapTopicsSelector topicsSelector,
    ITopicQuestionDistributionBuilder topicQuestionDistributionBuilder) : ITopicQuestionComposer
{
    public async Task<Result<RoadmapTestResult>> GenerateRoadmapTestQuestions(List<Node> nodes, List<Edge> edges, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var topics = nodes.Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
        //var topicsForTesting = topicsSelector.SelectCoreTopics(nodes, edges, config.NumberOfQuestions ?? RoadmapTestConstants.DefaultNumberOfQuestions);
        //var topicTestPlan = topicQuestionDistributionBuilder.BuildTopicQuestionDistribution(topicsForTesting, config, ct);
        //var targetTopics = FilterTopicsByAllocatedQuestions(topicTestPlan, topicsForTesting);
        var structureMeta = AnalyzeStructure(new Graph { Nodes = nodes, Edges = edges });
        var questionsLimit = (int)Math.Round(Math.Min(nodes.Count * 0.25, RoadmapTestConstants.MaxNumberOfQuestions));
        var targetTopicIds = SelectQuestions(structureMeta, questionsLimit);
        var targetTopics = topics.Where(t => targetTopicIds.Contains(t.Id)).ToList();
        var topicQuestionsCreationSettings = targetTopics.ToDictionary(t => t.Id, t => GetTopicQuestionsGenerationSettings(t, null, config.DifficultyLevel));
        var generateTestQuestions = await topicQuestionsGenerator.GenerateTopicsQuestions(topicQuestionsCreationSettings.Values.ToList(), ct);
        var generateTestQuestionsDict = generateTestQuestions.GroupBy(t => t.Id).ToDictionary(g => g.Key, g => g.ToList());
        return Result.Success(new RoadmapTestResult(
            targetTopics.ToDictionary(
                t => t.Id,
                t => new TopicTestBundle(
                    Questions: generateTestQuestionsDict.GetOrDefault(t.Id, []),
                    CreationSettings: topicQuestionsCreationSettings.GetOrDefaultAsNullable(t.Id)?.Setting
                ))));
    }

    private List<string> SelectQuestions(
        Dictionary<string, NodeMeta> meta,
        int budget)
    {
        var selected = new List<string>();
        var covered = new HashSet<string>();

        while (selected.Count < budget)
        {
            NodeMeta best = null;
            double bestScore = double.MinValue;

            foreach (var node in meta.Values)
            {
                if (selected.Contains(node.Node.Id))
                    continue;

                var score = MarginalGain(node, covered); // todo: logic behind the gain
                if (score > bestScore)
                {
                    bestScore = score;
                    best = node;
                }
            }

            if (best == null)
                break;

            selected.Add(best.Node.Id);

            covered.Add(best.Node.Id);
            covered.UnionWith(best.Ancestors);
            covered.UnionWith(best.Descendants);
        }

        return selected;
    }

    private double MarginalGain(NodeMeta node, HashSet<string> covered)
    {
        int novel = 0;

        foreach (var d in node.Descendants)
            if (!covered.Contains(d)) novel++;

        foreach (var a in node.Ancestors)
            if (!covered.Contains(a)) novel++;

        return node.StructuralInfluence * (1 + novel);
    }
    private Dictionary<string, NodeMeta> AnalyzeStructure(Graph graph)
    {
        var meta = BuildMetadata(graph);

        ComputeDepths(meta, graph.Edges.ToList());
        ComputeReachability(meta, graph.Edges.ToList());
        ComputeStructuralInfluence(meta);

        return meta;
    }


    private Dictionary<string, NodeMeta> BuildMetadata(Graph graph)
    {
        var map = graph.Nodes.ToDictionary(
            n => n.Id,
            n => new NodeMeta(n)
        );

        foreach (var edge in graph.Edges)
        {
            if (!map.ContainsKey(edge.Source) || !map.ContainsKey(edge.Target))
                continue; // or throw?
            map[edge.Target].InDegree++;
            map[edge.Source].OutDegree++;
        }

        return map;
    }

    private void ComputeDepths(
        Dictionary<string, NodeMeta> meta,
        List<Edge> edges)
    {
        var queue = new Queue<NodeMeta>(
            meta.Values.Where(n => n.InDegree == 0)
        );

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var edge in edges.Where(e => e.Source == current.Node.Id))
            {
                var target = meta[edge.Target];
                target.Depth = Math.Max(target.Depth, current.Depth + 1);

                target.InDegree--;
                if (target.InDegree == 0)
                    queue.Enqueue(target);
            }
        }
    }

    private void ComputeReachability(
    Dictionary<string, NodeMeta> meta,
    List<Edge> edges)
    {
        var forward = edges
            .GroupBy(e => e.Source)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Target).ToList());

        var backward = edges
            .GroupBy(e => e.Target)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Source).ToList());

        foreach (var node in meta.Values)
        {
            DFS(node.Node.Id, node.Descendants, forward);
            DFS(node.Node.Id, node.Ancestors, backward);
        }
    }

    private void DFS(
        string start,
        HashSet<string> visited,
        Dictionary<string, List<string>> adjacency)
    {
        if (!adjacency.TryGetValue(start, out var next))
            return;

        foreach (var n in next)
        {
            if (visited.Add(n))
                DFS(n, visited, adjacency);
        }
    }

    private void ComputeStructuralInfluence(Dictionary<string, NodeMeta> meta)
    {
        foreach (var node in meta.Values)
        {
            node.StructuralInfluence =
                Math.Log(node.Descendants.Count + 1) *
                (node.OutDegree + 1) /
                Math.Log(node.Depth + 2); // todo: clarify this formula
        }
    }

    private static List<Topic> FilterTopicsByAllocatedQuestions(
        Dictionary<string, TopicQuestionPlan> topicTestPlan,
        List<Topic> topics)
    {
        return topicTestPlan
            .Where(ta => ta.Value.QuestionsCount > 0)
            .OrderByDescending(ta => ta.Value.QuestionsCount)
            .Join(
                topics,
                ta => ta.Key,
                t => t.Id,
                (ta, t) => t)
            .ToList();
    }
    private static (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicQuestionsGenerationSettings(Topic topic, TopicQuestionPlan analysis, string difficultyLevel)
    {
        return (topic, new TopicQuestionsSettingDto
        {
            DifficultyLevel = difficultyLevel.FromDifficultyString(),
            QuestionsCount = analysis?.QuestionsCount ?? 1,
            Types = RoadmapTestConstants.SupportedQuestionTypes.ToList(),
        });
    }
}