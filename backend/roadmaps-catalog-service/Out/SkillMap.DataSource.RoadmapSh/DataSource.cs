using Microsoft.VisualBasic;
using SkillMap.Application.OutPorts.DataSource;
using SkillMap.DataSource.RoadmapSh.Api;
using SkillMap.DataSource.RoadmapSh.Configs;
using SkillMap.DataSource.RoadmapSh.Mappers;
using SkillMap.Shared;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Resources;
using System.Runtime.InteropServices;
using System.Transactions;
using System;
using System.Text;

namespace SkillMap.DataSource.RoadmapSh;

internal class DataSource : IRoadmapDataSource
{
    public async Task<(List<NodeDto> Nodes, List<EdgeDto<NodeDto>> Edges)> GetRoadmapSource(DataSourceConfig config, CancellationToken ct = default)
    {
        if (config == null || config?.RoadmapName == null)
        {
            throw new ArgumentNullException("Empty configs");
        }

        var roadmapName = config.RoadmapName;
        var roadmapResponse = await GetRoadmapResponseAsync(roadmapName, ct);
        if (roadmapResponse == null)
        {
            throw new ArgumentNullException("Empty roadmap response");
        }

        var migrationMappings = await GetMigrationMappings(roadmapName, ct);

        var nodes = roadmapResponse.Nodes
            .Select(x => x.MapToNodeDto())
            .Where(x => x.IsValidNode())
            .ToList();
        var mappingNodes = GetNodesFromMapping(migrationMappings, nodes, roadmapName)
            .Where(x => x.IsValidNode())
            .ToList();
        nodes.AddRange(mappingNodes);

        var nodesByExId = nodes
            .GroupBy(n => n.ExternalId)
            .ToDictionary(n => n.Key, n => n.FirstOrDefault());

        var edges = roadmapResponse.Edges
            .Select(x => x.ToEdgeDto(nodesByExId))
            .Where(x => x.IsValidEdge())
            .ToList();
        var mappingEdges = GetEdgesFromMapping(migrationMappings, nodes)
            .Where(x => x.IsValidEdge())
            .ToList();
        edges.AddRange(mappingEdges);

        return (nodes, edges);
    }


    public async Task<List<FileDataDto>> GetFolderContent(DataSourceConfig config, CancellationToken ct = default)
    {
        if (config is not DescriptionsDataSourceConfig roadmapShConfig)
        {
            throw new ArgumentNullException("Invalid config");
        }

        var url = RoadmapShConstants.GetGitHubUrl(roadmapShConfig.Source);
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
        var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(ct);
        var files = content.DeserializeOrDefault<List<FileInfoResponse>>();

        var results = files.Select(f => f.ToFile()).ToList();
        var tasks = new List<Task>();
        foreach (var file in results)
        {
            tasks.Add(Task.Run(async () =>
            {
                var fileContent = await client.GetAsync(file.DownloadUrl, ct);
                fileContent.EnsureSuccessStatusCode();
                var fileContentBytes = await fileContent.Content.ReadAsByteArrayAsync(ct);
                file.Content = fileContentBytes;
            }, ct));
        }

        await Task.WhenAll(tasks);

        return results;
    }

    public async Task<(string Description, List<ResourceDto> ResourceDtos)> ParseFileContent(FileDataDto fileDataDto, CancellationToken ct)
    {
        try
        {
            var fileContent = UTF8Encoding.UTF8.GetString(fileDataDto.Content);

            var lines = fileContent.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var description = lines[1].Trim();

            if (lines.Length < 4)
            {
                return (description, null);
            }

            var resources = lines[3].Split(new[] { "\n-" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Select(r =>
                {
                    try
                    {
                        var parts = r.Split(new[] { "](" }, StringSplitOptions.RemoveEmptyEntries);
                        var link = parts[1].Trim(')');
                        var firstDot = parts[0].IndexOf('@');
                        var lastDot = parts[0].LastIndexOf('@');
                        var type = parts[0].Substring(firstDot + 1, lastDot - firstDot - 1).Trim();
                        var title = parts[0].Substring(lastDot + 1).Trim();

                        return new ResourceDto
                        {
                            Title = title,
                            Type = type switch
                            {
                                "article" => ResourceType.Article,
                                "video" => ResourceType.Video,
                                "book" => ResourceType.Book,
                                "course" => ResourceType.Course,
                                _ => ResourceType.Article
                            },
                            Link = link,
                        };
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }).Where(r => r != null).ToList();

            return (description, resources);
        }
        catch (Exception ex)
        {
            // Handle file parsing error
            // Console.WriteLine($"Error parsing file content: {ex.Message}");
            return (null, null);
        }
    }
    private async Task<RoadmapResponse> GetRoadmapResponseAsync(string roadmapName, CancellationToken ct = default)
    {
        using var client = new HttpClient();
        var url = RoadmapShConstants.GetBaseUrl(roadmapName);
        var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(ct);
        return content.DeserializeOrDefault<RoadmapResponse>();
    }
    private async Task<Dictionary<string, string>> GetMigrationMappings(string roadmapName, CancellationToken ct = default)
    {
        using var client = new HttpClient();
        var url = RoadmapShConstants.GetMigrationMappingUrl(roadmapName);
        var response = await client.GetAsync(url, ct);
        // response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<string, string>();
        }
        var content = await response.Content.ReadAsStringAsync(ct);
        return content.DeserializeOrDefault<Dictionary<string, string>>();
    }
    private List<NodeDto> GetNodesFromMapping(Dictionary<string, string> mappings, List<NodeDto> nodes, string roadmapName)
    {
        var mappedNodes = new List<NodeDto>();
        if (mappings == null || mappings.Count < 1)
        {
            return mappedNodes;
        }
        var initialNodesDict = nodes
        .GroupBy(n => n.Title.Replace(" ", "").ToLower().Trim())
        .ToDictionary(n => n.Key, n => n.FirstOrDefault());
        var initialNodesByExId = nodes
        .ToDictionary(n => n.ExternalId, n => n);
        var mappingNodesWithExId = mappings
        .GroupBy(m => m.Key.Split(":").Last())
        .ToDictionary(m => m.Key, m => m.First().Value);
        foreach (var mapNode in mappingNodesWithExId)
        {
            if (!initialNodesByExId.ContainsKey(mapNode.Value))
            {
                string type = NodeType.SubTopic;
                if (mappings.ContainsKey(mapNode.Key))
                {
                    type = NodeType.Topic;
                }
                mappedNodes.Add(new NodeDto
                {
                    Title = mapNode.Key.Replace("-", " "),
                    ExternalId = mapNode.Value,
                    Type = type
                });
            }
            else
            {
                var isTopic = mappings.ContainsKey(mapNode.Key);
                mappedNodes.Add(new NodeDto
                {
                    Title = mapNode.Key.Replace("-", " "),
                    ExternalId = isTopic ? mapNode.Value : $"{roadmapName}_{mapNode.Key}",
                    Type = isTopic ? NodeType.Topic : NodeType.SubTopic,
                });
            }
        }
        return mappedNodes;
    }

    private List<EdgeDto<NodeDto>> GetEdgesFromMapping(Dictionary<string, string> mappings, List<NodeDto> nodes)
    {
        var mappedEdges = new List<EdgeDto<NodeDto>>();
        if (mappings == null || mappings.Count < 1)
        {
            return mappedEdges;
        }

        var allMappingNodes = nodes
            .GroupBy(n => n.Title
            .Replace(" ", "")
            .Replace("/", "")
            .Replace("of", "")
            .ToLower().Trim())
            .ToDictionary(n => n.Key, n => n.First());
        var nodesByExId = nodes
            .GroupBy(n => n.ExternalId)
            .ToDictionary(n => n.Key, n => n.FirstOrDefault());

        var rootTopics = new Stack<NodeDto>();

        foreach (var edge in mappings)
        {
            var topics = edge.Key.Split(":").Select(t => t.Replace("-", "").ToLower().Trim()).ToList();
            if (topics.Count <= 1)
            {
                if (rootTopics.Count == 0)
                {
                    if (allMappingNodes.TryGetValue(topics[0], out var rootNode1))
                    {
                        rootTopics.Push(rootNode1);
                    }
                    continue;
                }
                var previousNode = rootTopics.Pop();
                var targetNode = allMappingNodes.GetOrDefault(topics[0]);
                if (targetNode == null)
                {
                    targetNode = nodesByExId.GetOrDefault(edge.Value);
                    //var similarName = StringSimilarity.FindMostSimilar(topics[0], allMappingNodes.Keys.ToList());
                    //if (similarName != null)
                    //{
                    //    targetNode = allMappingNodes[similarName];
                    //}
                    //else
                    //{
                    //    targetNode = nodesByExId.GetOrDefault(edge.Value);
                    //}
                }
                mappedEdges.Add(new EdgeDto<NodeDto>
                {
                    Source = previousNode,
                    Target = targetNode,
                });

                rootTopics.Push(targetNode);

                continue;
            }

            for (int i = 0; i < topics.Count - 1; i++)
            {
                var source = topics[i];
                var target = topics[i + 1];
                allMappingNodes.TryGetValue(source, out var sourceNode);
                allMappingNodes.TryGetValue(target, out var targetNode);

                if (sourceNode != null && targetNode != null && !mappedEdges.Any(e => e.Source.ExternalId == sourceNode.ExternalId && e.Target.ExternalId == targetNode.ExternalId))
                {
                    mappedEdges.Add(new EdgeDto<NodeDto>
                    {
                        Source = sourceNode,
                        Target = targetNode,
                    });
                }
            }
        }

        return mappedEdges;
    }
}
