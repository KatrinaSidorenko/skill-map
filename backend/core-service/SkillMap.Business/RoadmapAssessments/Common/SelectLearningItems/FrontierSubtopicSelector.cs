using SkillMap.Core.Constants;

namespace SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;

internal sealed class SimpleNodeMeta
{
    public string Id { get; }
    public HashSet<string> Ancestors { get; } = new();
    public HashSet<string> Descendants { get; } = new();

    public SimpleNodeMeta(string id) => Id = id;
}

/// <summary>
/// Выбирает подтемы на границе знаний (Frontier).
/// Ищет темы, которые доступны для изучения (пререквизиты сданы), но еще не пройдены,
/// отдавая приоритет тем, которые открывают наибольшее количество новых тем (максимальное влияние).
/// </summary>
internal static class FrontierSubtopicSelector
{
    internal static List<LeaningItemAssessment> Select(
        List<LeaningItemAssessment> frontierPool,
        List<LeaningItemAssessment> topics,
        List<LearningItemsConnectionAssessment> topicDependencies,
        Dictionary<string, List<string>> subtopicToTopicMap,
        int budget,
        Random rnd)
    {
        if (budget <= 0) return [];

        // 1. Фиксируем то, что студент уже знает (или мы предполагаем, что знает)
        var masteredTopicIds = topics
            .Where(t => t.Status == LearningStatus.Completed || t.Assumption == AssessmentAssumption.AssumedCompleted)
            .Select(t => t.Id)
            .ToHashSet();

        // 2. Строим простой граф (нам нужны только предки для проверки доступности и потомки для веса)
        var graphMeta = BuildGraphReachability(topics, topicDependencies);

        // 3. Выбираем лучшие темы на границе знаний
        var bestFrontierTopicIds = RunSimplifiedGreedySetCover(graphMeta, budget, masteredTopicIds);

        // 4. Достаем подтемы, принадлежащие выбранным темам
        var fromBestTopics = frontierPool
            .Where(s => subtopicToTopicMap.TryGetValue(s.Id, out var parentTopics) &&
                        parentTopics.Any(t => bestFrontierTopicIds.Contains(t)))
            .OrderBy(_ => rnd.Next())
            .ToList();

        var selected = fromBestTopics.Take(budget).ToList();

        // 5. Fallback: добиваем случайными темами из пула, если граф выдал меньше, чем нужно
        if (selected.Count < budget)
        {
            var remaining = frontierPool
                .Except(selected)
                .OrderBy(_ => rnd.Next())
                .Take(budget - selected.Count);

            selected.AddRange(remaining);
        }

        return selected;
    }

    private static List<string> RunSimplifiedGreedySetCover(
        Dictionary<string, SimpleNodeMeta> meta,
        int budget,
        HashSet<string> mastered)
    {
        var selected = new List<string>();
        var covered = new HashSet<string>(); // Темы, неопределенность которых уже "покрыта" нашими вопросами

        while (selected.Count < budget)
        {
            // ФРОНТИР: Тема еще не изучена, но ВСЕ её предки (пререквизиты) уже изучены
            var frontier = meta.Values
                .Where(n => !mastered.Contains(n.Id))
                .Where(n => n.Ancestors.IsSubsetOf(mastered))
                .ToList();

            if (frontier.Count == 0) break; // Достигли конца графа или застряли

            // ЖАДНЫЙ ВЫБОР: Берем тему, которая имеет больше всего ЕЩЕ НЕ ПОКРЫТЫХ потомков
            var bestTopic = frontier
                .OrderByDescending(n => n.Descendants.Count(d => !covered.Contains(d)))
                .First();

            selected.Add(bestTopic.Id);

            // Оптимистично помечаем тему как "изученную", чтобы в следующем цикле 
            // алгоритм шагнул дальше вглубь графа (сдвигаем границу)
            mastered.Add(bestTopic.Id);

            // Помечаем эту тему и всех её потомков как "покрытых" нашим вопросом, 
            // чтобы на следующем шаге выбрать тему из другой ветки графа
            covered.Add(bestTopic.Id);
            covered.UnionWith(bestTopic.Descendants);
        }

        return selected;
    }

    // -------------------------------------------------------------------------
    // Построение графа (Поиск предков и потомков)
    // -------------------------------------------------------------------------

    private static Dictionary<string, SimpleNodeMeta> BuildGraphReachability(
        List<LeaningItemAssessment> topics,
        List<LearningItemsConnectionAssessment> edges)
    {
        var meta = topics.ToDictionary(t => t.Id, t => new SimpleNodeMeta(t.Id));

        var forward = edges.GroupBy(e => e.FromId).ToDictionary(g => g.Key, g => g.Select(e => e.ToId).ToList());
        var backward = edges.GroupBy(e => e.ToId).ToDictionary(g => g.Key, g => g.Select(e => e.FromId).ToList());

        foreach (var node in meta.Values)
        {
            DFS(node.Id, node.Descendants, forward);
            DFS(node.Id, node.Ancestors, backward);
        }

        return meta;
    }

    private static void DFS(string start, HashSet<string> visited, Dictionary<string, List<string>> adjacency)
    {
        if (!adjacency.TryGetValue(start, out var neighbors)) return;

        foreach (var neighbor in neighbors)
        {
            if (visited.Add(neighbor)) DFS(neighbor, visited, adjacency);
        }
    }
}