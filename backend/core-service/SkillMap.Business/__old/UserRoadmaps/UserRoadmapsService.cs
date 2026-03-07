using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Business.__old.UserRoadmaps.Models;
using SkillMap.Business.Abstractions;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.RoadmapBookmarks;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.__old.UserRoadmaps;

public class UserRoadmapsService(IRepository<RoadmapBookmark> userRoadmapsRepository, IRoadmapService roadmapService) : IUserRoadmapsService
{
    public async Task<Result<bool>> LinkRoadmap(long userId, string roadmapId, CancellationToken ct, bool isOwner = false)
    {
        var dbUserRoadmap = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (dbUserRoadmap is not null)
        {
            dbUserRoadmap.IsActive = true;
            await userRoadmapsRepository.UpdateAsync(dbUserRoadmap, ct);
            await userRoadmapsRepository.SaveChangesAsync(ct);
            return Result.Success(true);
        }

        var userRoadmap = new RoadmapBookmark
        {
            UserId = userId,
            RoadmapId = roadmapId,
            IsActive = true,
            IsAuthor = isOwner
        };

        await userRoadmapsRepository.AddAsync(userRoadmap, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult)
        {
            return ResultType.FailedToAddRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }

    public async Task<Result<UserRoadmapDto>> GetUserRoadmap(long userId, string roadmapId, CancellationToken ct, bool? isActive = null)
    {
        var dbUserRoadmap = await userRoadmapsRepository.GetFirstOrDefaultAsync(
        ur => ur.UserId == userId && ur.RoadmapId == roadmapId && (!isActive.HasValue || ur.IsActive == isActive.Value), ct: ct);
        if (dbUserRoadmap is null)
        {
            return ResultType.UserRoadmapNotFound<UserRoadmapDto>(userId, roadmapId);
        }

        return Result.Success(dbUserRoadmap.ToUserRoadmapDto());
    }

    public async Task<Result<List<UserRoadmapDto>>> GetUserSavedRoadmaps(long userId, CancellationToken ct)
    {
        var dbUserRoadmaps = await userRoadmapsRepository.GetAllAsync(ur => ur.UserId == userId && ur.IsActive, ct: ct);
        var dtos = dbUserRoadmaps.Select(ur => ur.ToUserRoadmapDto()).ToList();
        return Result.Success(dtos);
    }

    public async Task<Result<string>> CreateUserRoadmap(long userId, PlainRoadmapDto roadmapDto, CancellationToken ct)
    {
        var createRoadmapResult = await roadmapService.CreateRoadmap(roadmapDto, ct);
        if (!createRoadmapResult.IsSuccessful)
        {
            return ResultType.FailedToCreateUserRoadmap<string>(userId, createRoadmapResult.Message);
        }
        var linkRoadmapResult = await LinkRoadmap(userId, createRoadmapResult.Data, ct, true);
        if (!linkRoadmapResult.IsSuccessful)
        {
            return ResultType.FailedToCreateUserRoadmap<string>(userId, linkRoadmapResult.Message);
        }
        return Result.Success(createRoadmapResult.Data);
    }

    public async Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetUserCreatedRoadmaps(long userId, SearchingParams @params, CancellationToken ct)
    {
        var dbUserRoadmaps = await userRoadmapsRepository.GetAllAsync(ur => ur.UserId == userId && ur.IsAuthor && ur.IsActive, ct: ct);
        var roadmapIds = dbUserRoadmaps.Select(ur => ur.RoadmapId).ToList();
        var plainRoadmapsResult = await roadmapService.GetPlainRoadmapsByIds(roadmapIds, @params, ct, excludePrivate: false);
        if (!plainRoadmapsResult.IsSuccessful)
        {
            return ResultType.FailedToGetUserRoadmaps<PaginationResult<List<PlainRoadmapDto>>>(userId);
        }
        return plainRoadmapsResult;
    }

    public async Task<Result<bool>> RemoveRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmap = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (dbUserRoadmap is null)
        {
            return ResultType.UserRoadmapNotFound<bool>(userId, roadmapId);
        }

        dbUserRoadmap.IsActive = false;
        await userRoadmapsRepository.UpdateAsync(dbUserRoadmap, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult)
        {
            return ResultType.FailedToRemoveRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }
    public async Task<Result<PlainRoadmapDto>> GetCreatedUserRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmap = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId && ur.IsAuthor && ur.IsActive, ct: ct);
        if (dbUserRoadmap is null)
        {
            return ResultType.UserRoadmapNotFound<PlainRoadmapDto>(userId, roadmapId);
        }
        var plainRoadmapsResult = await roadmapService.GetPlainRoadmapsByIds(new List<string> { roadmapId }, new SearchingParams("", new PaginationParams(1, 1)), ct, excludePrivate: false);
        return !plainRoadmapsResult.IsSuccessful || plainRoadmapsResult.Data.Result.Count == 0
     ? ResultType.FailedToUpdateUserRoadmap<PlainRoadmapDto>(userId, roadmapId)
     : Result.Success(plainRoadmapsResult.Data.Result.FirstOrDefault());
    }
}