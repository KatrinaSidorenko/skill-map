using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Common;

internal sealed class TarjanSccDetector
{
    private const int Unvisited = -1;

    private int _index;
    private readonly int[] _visited;
    private readonly int[] _lowLinks;
    private readonly bool[] _onStack;
    private readonly Stack<int> _stack;
    private readonly List<List<int>> _sccComponents;
    private readonly List<List<int>> _adjacencyList;
    private readonly List<LearningItemSnapshot> _nodes;

    public TarjanSccDetector(
        List<LearningItemSnapshot> nodes,
        List<LearningItemsConnectionSnapshot> edges)
    {
        _nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));

        var nodeToIndex = nodes
            .Select((n, i) => (n.Id, i))
            .ToDictionary(x => x.Id, x => x.i);

        int count = nodes.Count;
        _visited = new int[count];
        _lowLinks = new int[count];
        _onStack = new bool[count];
        _stack = new Stack<int>();
        _sccComponents = [];

        _adjacencyList = Enumerable.Range(0, count).Select(_ => new List<int>()).ToList();

        foreach (var edge in edges ?? [])
        {
            if (nodeToIndex.TryGetValue(edge.FromId, out int src) &&
                nodeToIndex.TryGetValue(edge.ToId, out int dst))
            {
                _adjacencyList[src].Add(dst);
            }
        }
    }

    public TarjanSccDetector(RoadmapSnapshot snapshot)
        : this(snapshot?.LearningItems ?? [], snapshot?.LearningItemsConnections ?? []) { }

    public List<List<LearningItemSnapshot>> FindStronglyConnectedComponents()
    {
        Array.Fill(_visited, Unvisited);
        _index = 0;

        for (int v = 0; v < _nodes.Count; v++)
        {
            if (_visited[v] == Unvisited)
                StrongConnect(v);
        }

        return _sccComponents
            .Select(component => component.Select(i => _nodes[i]).ToList())
            .ToList();
    }

    private void StrongConnect(int v)
    {
        _visited[v] = _index;
        _lowLinks[v] = _index;
        _index++;

        _stack.Push(v);
        _onStack[v] = true;

        foreach (int neighbor in _adjacencyList[v])
        {
            if (_visited[neighbor] == Unvisited)
            {
                StrongConnect(neighbor);
                _lowLinks[v] = Math.Min(_lowLinks[v], _lowLinks[neighbor]);
            }
            else if (_onStack[neighbor])
            {
                _lowLinks[v] = Math.Min(_lowLinks[v], _visited[neighbor]);
            }
        }

        if (_lowLinks[v] == _visited[v])
        {
            var component = new List<int>();
            int w;
            do
            {
                w = _stack.Pop();
                _onStack[w] = false;
                component.Add(w);
            }
            while (w != v);

            _sccComponents.Add(component);
        }
    }
}
