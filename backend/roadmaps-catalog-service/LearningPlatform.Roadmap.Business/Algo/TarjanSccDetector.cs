using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace LearningPlatform.Roadmap.Business.Algo;

public class TarjanSccDetector
{
    private const int Unvisited = -1;

    private int _index;
    private int[] _visited;
    private int[] _lowLinks;
    private bool[] _onStack;
    private Stack<int> _stack;
    private List<List<int>> _adjacencyList;
    private List<List<int>> _sccComponents;
    private readonly Graph _graph;
    private readonly List<NodeDto> _nodes;

    public TarjanSccDetector(Graph graph)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        var (nodes, adjacencyList) = graph.ToIndexedGraph();
        _adjacencyList = adjacencyList;
        _nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));

        int nodeCount = nodes.Count;
        _visited = new int[nodeCount];
        _lowLinks = new int[nodeCount];
        _onStack = new bool[nodeCount];
        _stack = new Stack<int>();
        _sccComponents = new List<List<int>>();
    }

    public List<List<NodeDto>> FindStronglyConnectedComponents()
    {
        Array.Fill(_visited, Unvisited);
        _index = 0;

        for (int v = 0; v < _nodes.Count; v++)
        {
            if (_visited[v] == Unvisited)
            {
                StrongConnect(v);
            }
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

        foreach (var neighbor in _adjacencyList[v])
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

