using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Helpers;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Business.Abstractions;
using SkillMap.Business.ModifiedRoadmaps.Mappers;
using SkillMap.Business.ModifiedRoadmaps.Models;
using SkillMap.Business.Roadmaps.Helpers;
using SkillMap.Business.Roadmaps.Mappers;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Gzip;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Roadmaps;

public class CustomizedRoadmapsService(
    IRoadmapService roadmapService, 
    IUserRoadmapsService userRoadmapsService, 
    IRepository<RoadmapModification> modificationsRepository,
    IServiceProvider serviceProvider,
    IRepository<RoadmapSnapshot> snapshotRepository) : ICustomizedRoadmapsService
{
    private const int MaxModificationsCount = 5;

    public Task<Result<NodeResponse>> Create(long userId, string roadmapId, CreateLearningItemMetadata itemMetadata, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteRoadmap(long userId, string roadmapId, string itemId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PaginationResult<List<PlainRoadmapWithDetailsDto>>>> GetPlainRoadmapsWithUserMetadata(long userId, SearchingParams @params, CancellationToken ct)
    {
        var userRoadmapsResult = await userRoadmapsService.GetUserRoadmaps(userId, ct);
        if (!userRoadmapsResult.IsSuccessful)
        {
            return ResultType.UserRoadmapNotFound<PaginationResult<List<PlainRoadmapWithDetailsDto>>>(userId);
        }

        var userRoadmapIds = userRoadmapsResult.Data.Select(ur => ur.RoadmapId).ToHashSet();
        var paginatedRoadmapsResult = await roadmapService.GetPlainRoadmapsByIds([.. userRoadmapIds], @params, ct);
        if (!paginatedRoadmapsResult.IsSuccessful)
        {
            return ResultType.RoadmapNotFound<PaginationResult<List<PlainRoadmapWithDetailsDto>>>("");
        }

        var allRoadmaps = paginatedRoadmapsResult.Data.Result;

        // add calculate logic for progress
        // status of roadmap

        return Result.Success(new PaginationResult<List<PlainRoadmapWithDetailsDto>>
        {
            Result = allRoadmaps.Select(r => r.ToPlainRoadmapWithDetailsDto()).ToList(),
            TotalCount = paginatedRoadmapsResult.Data.TotalCount,
        });
    }

    // todo: create optimized query to db for target node types in order to exclude resources
    private async Task<Result<double>> GetRoadmapProgress(long userId, string roadmapId, CancellationToken ct)
    {
        // get full roadmap (set of nodes)
        var roadmap = new List<NodeDto>();
        roadmap = roadmap.Where(r => r.IsTopic() || r.IsSubTopic()).ToList(); // in order to exclude resources 

        // what about the deleted items?
        // get user modifications for the roadmap

        // calculate the total count of items
        // calculate the completed count of items

        // progress = completed / total * 100
        throw new NotImplementedException();
    }

    private async Task<string> GetRoadmapStatus(long userId, string roadmapId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SavedUerRoadmap>> GetRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var sourceRoadmapResult = await roadmapService.GetRoadmapById(roadmapId, ct); // todo: this is already without resources 
        if (!sourceRoadmapResult.IsSuccessful)
        {
            return ResultType.RoadmapNotFound<SavedUerRoadmap>(roadmapId);
        }

        var sourceRoadmap = sourceRoadmapResult.Data;
        
        return Result.Success(new SavedUerRoadmap
        {
            Id = sourceRoadmap.Id,
            Title = sourceRoadmap.Title,
            Description = sourceRoadmap.Description,
            Status = LearningStatus.NotStarted.ToString(),
            Progress = 0,
            Nodes = sourceRoadmap.Nodes
                .Select(n => new ModifiedNode
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    Status = LearningStatus.NotStarted.ToString(),
                }).ToList(),
            Edges = sourceRoadmap.Edges,
        });
    }

    public async Task<Result<bool>> SaveLearningItemChange(long userId, string roadmapId, LearningItemChange item, CancellationToken ct)
    {
        var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct);
        if (!userRoadmapResult.IsSuccessful)
        {
            return ResultType.UserRoadmapNotFound<bool>(userId, roadmapId);
        }

        var action = new RoadmapModification
        {
            UserRoadmapId = userRoadmapResult.Data.Id,
            ExternalItemId = item.Id,
            Metadata = item.SerializeOrDefault(),
            Action = ModificationAction.SnapshotUpdate,
        };

        await modificationsRepository.AddAsync(action, ct);
        var saveResult = await modificationsRepository.SaveChangesAsync(ct);
        if (!saveResult.IsSuccessful)
        {
            return ResultType.FailedToApplyModifications<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }

    public Task<Result<bool>> UpdateStatus(long userId, string roadmapId, string itemId, UpdateStatusMetadata metadata, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    //private async Task EnsureSnapshot(long userRoadmapId, string roadmapId, CancellationToken ct)
    //{
    //    using var scope = serviceProvider.CreateAsyncScope();
    //    var roadmapService = scope.ServiceProvider.GetRequiredService<IRoadmapService>();
    //    var modificationsRepository = scope.ServiceProvider.GetRequiredService<IRepository<RoadmapModification>>();
    //    var snapshotRepository = scope.ServiceProvider.GetRequiredService<IRepository<RoadmapSnapshot>>();

    //    var latestSnapshotResult = await snapshotRepository.GetAllAsync(
    //        s => s.UserRoadmapId == userRoadmapId,
    //        orderBy: q => q.OrderByDescending(s => s.CreatedAt),
    //        ct: ct);

    //    var targetModifications = (List<RoadmapModification>)null;
    //    var targetNodes = (List<NodeResponse>)null;
    //    var targetEdges = (List<(string Source, string Target)>)null;
    //    if (!latestSnapshotResult.IsSuccessful || !latestSnapshotResult.Data.Any())
    //    {
    //        var fullRoadmapResult = await roadmapService.GetFullPlainRoadmap(roadmapId, ct);
    //        var fullRoadmap = fullRoadmapResult.Data;

    //        var modificationsResult = await modificationsRepository.GetAllAsync(
    //            m => m.UserRoadmapId == userRoadmapId,
    //            orderBy: q => q.OrderByDescending(m => m.CreatedAt),
    //            ct: ct);

    //        targetNodes = fullRoadmap.Nodes.Select(n => n.Map()).ToList();
    //        targetEdges = fullRoadmap.Edges.Select(e => (Source: e.Source, Target: e.Target)).ToList();
    //        targetModifications = modificationsResult.Data?.ToList();
    //    }
    //    else
    //    {
    //        var snapshot = latestSnapshotResult.Data.First();
    //        var roadmap = await snapshot.Content.InGzipJsonObjectUtf8<Tree>(ct);
    //        var modificationsResult = await modificationsRepository.GetAllAsync(
    //            m => m.UserRoadmapId == userRoadmapId && m.CreatedAt > snapshot.CreatedAt,
    //            orderBy: q => q.OrderBy(m => m.CreatedAt),
    //            ct: ct);

    //        if (!modificationsResult.IsSuccessful || modificationsResult.Data.Count() < MaxModificationsCount) { return; }

    //        targetModifications = modificationsResult.Data.ToList();
    //        targetNodes = roadmap.Nodes;
    //        targetEdges = roadmap.Edges;
    //    }

    //    if (targetModifications == null || targetModifications?.Count <= 0) {  return; }

    //    var customizedGraph = (targetNodes, targetEdges).ApplyModifications(targetModifications.ToList());
    //    customizedGraph.ApplySortingAndIndexing();

    //    var content = new Tree { Nodes = customizedGraph.Item1, Edges = customizedGraph.Item2, };
    //    var newSnapshot = new RoadmapSnapshot { UserRoadmapId = userRoadmapId, Content = await content.GzipJsonObjectUtf8(ct), };

    //    await snapshotRepository.AddAsync(newSnapshot, ct);
    //    var saveResult = await snapshotRepository.SaveChangesAsync(ct);
    //}

    //private async Task<Result<bool>> CreateModification(long userId, string roadmapId, RoadmapModification modification, CancellationToken ct)
    //{
    //    var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct);
    //    if (!userRoadmapResult.IsSuccessful)
    //    {
    //        return ResultType.UserRoadmapNotFound<bool>(userId, roadmapId);
    //    }

    //    var userRoadmap = userRoadmapResult.Data;
    //    modification.UserRoadmapId = userRoadmap.Id;

    //    await modificationsRepository.AddAsync(modification, ct);
    //    var saveResult = await modificationsRepository.SaveChangesAsync(ct);
    //    if (!saveResult.IsSuccessful)
    //    {
    //        return ResultType.FailedToDeleteRoadmap<bool>(userId, roadmapId);
    //    }

    //    _ = Task.Run(async () => await EnsureSnapshot(userRoadmapResult.Data.Id, roadmapId, ct), ct);

    //    return Result.Success(true);
    //}
    //public async Task<Result<bool>> DeleteRoadmap(long userId, string roadmapId, string itemId, CancellationToken ct)
    //{
    //    var action = new RoadmapModification
    //    {
    //        Action = ModificationAction.Delete,
    //        ExternalItemId = itemId,
    //    };

    //   return await CreateModification(userId, roadmapId, action, ct);
    //}

    //public async Task<Result<bool>> UpdateStatus(long userId, string roadmapId, string itemId, UpdateStatusMetadata metadata, CancellationToken ct)
    //{
    //    var action = new RoadmapModification
    //    {
    //        Action = ModificationAction.UpdateStatus,
    //        ExternalItemId = itemId,
    //        Metadata = metadata.SerializeOrDefault(),
    //    };

    //    return await CreateModification(userId, roadmapId, action, ct);
    //}

    //public async Task<Result<bool>> Update(long userId, string roadmapId, LearningItemSnapshot item, CancellationToken ct)
    //{
    //    var action = new RoadmapModification
    //    {
    //        Action = ModificationAction.SnapshotUpdate,
    //        ExternalItemId = item.Id,
    //        Metadata = item.SerializeOrDefault(),
    //    };

    //    return await CreateModification(userId, roadmapId, action, ct);
    //}

    //public async Task<Result<NodeResponse>> Create(long userId, string roadmapId, CreateLearningItemMetadata itemMetadata, CancellationToken ct)
    //{
    //    var id = Guid.NewGuid().ToString();
    //    var action = new RoadmapModification
    //    {
    //        Action = ModificationAction.Create,
    //        InnerItemId = id,
    //        ExternalItemId = id,
    //        Metadata = itemMetadata.SerializeOrDefault(),
    //    };

    //    var createResult = await CreateModification(userId, roadmapId, action, ct);
    //    if (!createResult.IsSuccessful)
    //    {
    //        return ResultType.FailedToCreateRoadmap<NodeResponse>(userId, roadmapId);
    //    }


    //    var node = new NodeResponse
    //    {
    //        Id = id,
    //        Title = itemMetadata.Title,
    //        Type = itemMetadata.Type,
    //        ParentId = itemMetadata.ParentId,
    //        Index = int.MaxValue,
    //        Description = itemMetadata.Description,
    //        Status = itemMetadata.Status.ToString(),
    //        Progress = 0,
    //    };

    //    return Result.Success(node);
    //}

    //public async Task<Result<double>> GetRoadmapProgress(long userId, string roadmapId, CancellationToken ct)
    //{
    //    var customizedRoadmapResult = await GetRoadmap(userId, roadmapId, ct);
    //    if (!customizedRoadmapResult.IsSuccessful)
    //    {
    //        return ResultType.RoadmapNotFound<double>(roadmapId);
    //    }

    //    var customizedRoadmap = customizedRoadmapResult.Data;
    //    var allNodes = customizedRoadmap.LearningItems.GetAllFlattenNodes().Where(n => !n.Type.IsResource()).ToList();
    //    var completedNodes = allNodes.Where(n => n.Status == LearningStatus.Completed.ToString()).ToList();

    //    return Result.Success(Math.Round((completedNodes.Count / (double)allNodes.Count) * 100));
    //}

    //public async Task<Result<List<RoadmapDto>>> GetPlainRoadmapsWithUserMetadata(long userId, CancellationToken ct)
    //{
    //    var userRoadmapsResult = await userRoadmapsService.GetUserRoadmaps(userId, ct);
    //    if (!userRoadmapsResult.IsSuccessful)
    //    {
    //        return ResultType.UserRoadmapNotFound<List<RoadmapDto>>(userId);
    //    }

    //    var userRoadmapIds = userRoadmapsResult.Data.Select(ur => ur.RoadmapId).ToList();
    //    var allRoadmapsResult = await roadmapService.GetRoadmaps(userRoadmapIds, ct);
    //    if (!allRoadmapsResult.IsSuccessful)
    //    {
    //        return ResultType.RoadmapNotFound<List<RoadmapDto>>("");
    //    }

    //    var allRoadmaps = allRoadmapsResult.Data;
    //    var roadmapsWithProgress = new List<RoadmapDto>();
    //    foreach (var roadmap in allRoadmaps)
    //    {
    //        var progressResult = await GetRoadmapProgress(userId, roadmap.Id, ct);
    //        if (!progressResult.IsSuccessful)
    //        {
    //            return ResultType.RoadmapNotFound<List<RoadmapDto>>(roadmap.Id);
    //        }

    //        var progress = progressResult.Data;
    //        roadmapsWithProgress.Add(new RoadmapDto
    //        {
    //            Id = roadmap.Id,
    //            Title = roadmap.Title,
    //            Type = NodeType.Roadmap,
    //            Progress = progress,
    //        });
    //    }

    //    return Result.Success(roadmapsWithProgress);
    //}

    //public async Task<Result<CustomizedUerRoadmap>> GetRoadmap(long userId, string roadmapId, CancellationToken ct)
    //{
    //    var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct);
    //    if (!userRoadmapResult.IsSuccessful)
    //    {
    //        return ResultType.UserRoadmapNotFound<CustomizedUerRoadmap>(userId, roadmapId);
    //    }

    //    var userRoadmap = userRoadmapResult.Data;
    //    var targetRoadmapResult = await GetTargetRoadmap(userRoadmap.Id, roadmapId, ct);
    //    if (!targetRoadmapResult.IsSuccessful)
    //    {
    //        return ResultType.RoadmapNotFound<CustomizedUerRoadmap>(roadmapId);
    //    }

    //    var (sourceNodes, sourceEdges, modificationsResult) = targetRoadmapResult.Data;
    //    var targetTree = (sourceNodes, sourceEdges);
    //    if (modificationsResult.Count > 0)
    //    {
    //        targetTree = (sourceNodes, sourceEdges).ApplyModifications(modificationsResult.ToList());
    //        targetTree.ApplySortingAndIndexing();
    //    }

    //    targetTree.FillChildren();
    //    var rootNode = targetTree.Item1.FirstOrDefault(n => n.Type.IsRoadmap());

    //    return Result.Success(new CustomizedUerRoadmap
    //    {
    //        Roadmap = new RoadmapDto
    //        {
    //            Id = rootNode.Id,
    //            Title = rootNode.Title,
    //            Type = NodeType.Roadmap,
    //        },
    //        LearningItems = targetTree
    //            .Item1
    //            .Where(n => !n.IsDeleted)
    //            .Where(n => n.Type.IsTopic())
    //            .OrderBy(n => n.Index)
    //            .Select(n => n.Map(rootNode.Id))
    //            .ToList(),
    //    });
    //}

    //private async Task<Result<(List<NodeResponse> Nodes, List<(string Source, string Target)> Edges, List<RoadmapModification> Modifications)>> GetTargetRoadmap(
    //    long userRoadmapId, 
    //    string roadmapId, 
    //    CancellationToken ct)
    //{

    //    var targetNodes = (List<NodeResponse>)null;
    //    var targetEdges = (List<(string Source, string Target)>)null;
    //    var targetModifications = (List<RoadmapModification>)null;
    //    var snapshot = await snapshotRepository.GetAllAsync(
    //        s => s.UserRoadmapId == userRoadmapId,
    //        orderBy: q => q.OrderByDescending(s => s.CreatedAt),
    //        count: 1,
    //        ct: ct);
    //    if (snapshot.IsSuccessful && snapshot.Data.Any())
    //    {
    //        var decompressedSnapshot = await snapshot.Data.First().Content.InGzipJsonObjectUtf8<Tree>(ct);
    //        var modifications = await modificationsRepository.GetAllAsync(
    //            m => m.UserRoadmapId == userRoadmapId && m.CreatedAt >= snapshot.Data.First().CreatedAt,
    //            orderBy: q => q.OrderBy(m => m.CreatedAt),
    //            ct: ct);
    //        targetModifications = modifications.Data?.ToList();
    //        targetNodes = decompressedSnapshot.Nodes;
    //        targetEdges = decompressedSnapshot.Edges;
    //    }
    //    else
    //    {
    //        var fullRoadmapResult = await roadmapService.GetFullPlainRoadmap(roadmapId, ct);
    //        if (!fullRoadmapResult.IsSuccessful)
    //        {
    //            return ResultType.RoadmapNotFound<(List<NodeResponse> Nodes, List<(string Source, string Target)> Edges, List<RoadmapModification> Modifications)>(roadmapId);
    //        }
    //        var fullRoadmap = fullRoadmapResult.Data;
    //        var modificationsResult = await modificationsRepository.GetAllAsync(m => m.UserRoadmapId == userRoadmapId);
    //        if (!modificationsResult.IsSuccessful)
    //        {
    //            return ResultType.FailedToGetModifications<(List<NodeResponse> Nodes, List<(string Source, string Target)> Edges, List<RoadmapModification> Modifications)>(userRoadmapId, roadmapId);
    //        }

    //        targetModifications = modificationsResult.Data?.ToList();
    //        targetNodes = fullRoadmap.Nodes.Select(n => n.Map()).ToList();
    //        targetEdges = fullRoadmap.Edges.Select(e => (Source: e.Source, Target: e.Target)).ToList();
    //    }


    //    return Result.Success((targetNodes, targetEdges, targetModifications));
    //}
}
