using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Business.Abstractions;
using SkillMap.Business.UserRoadmaps.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.UserRoadmaps;

public class UserRoadmapsService(IRepository<UserRoadmap> userRoadmapsRepository, IRoadmapService roadmapService) : IUserRoadmapsService
{
    public async Task<Result<bool>> LinkRoadmap(long userId, string roadmapId, CancellationToken ct, bool isOwner = false)
    {
        var dbUserRoadmapResult = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (dbUserRoadmapResult.HasData)
        {
            dbUserRoadmapResult.Data.IsActive = true;
            await userRoadmapsRepository.UpdateAsync(dbUserRoadmapResult.Data, ct);
            await userRoadmapsRepository.SaveChangesAsync(ct);
            return Result.Success(true);
        }

        var userRoadmap = new UserRoadmap
        {
            UserId = userId,
            RoadmapId = roadmapId,
            IsActive = true,
            IsOwner = isOwner
        };

        await userRoadmapsRepository.AddAsync(userRoadmap, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult.IsSuccessful)
        {
            return ResultType.FailedToAddRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }

    public async Task<Result<UserRoadmapDto>> GetUserRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmapResult = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (!dbUserRoadmapResult.HasData)
        {
            return ResultType.UserRoadmapNotFound<UserRoadmapDto>(userId, roadmapId);
        }

        return Result.Success(dbUserRoadmapResult.Data.ToUserRoadmapDto());
    }

    public async Task<Result<List<UserRoadmapDto>>> GetUserSavedRoadmaps(long userId, CancellationToken ct)
    {
        var dbUserRoadmapsResult = await userRoadmapsRepository.GetAllAsync(ur => ur.UserId == userId && ur.IsActive, ct: ct);
        if (!dbUserRoadmapsResult.IsSuccessful)
        {
            return ResultType.UserRoadmapNotFound<List<UserRoadmapDto>>(userId);
        }

        return Result.Success(dbUserRoadmapsResult.Data.Select(ur => ur.ToUserRoadmapDto()).ToList());
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
        var dbUserRoadmapsResult = await userRoadmapsRepository.GetAllAsync(ur => ur.UserId == userId && ur.IsOwner && ur.IsActive, ct: ct);
        if (!dbUserRoadmapsResult.IsSuccessful)
        {
            return ResultType.UserRoadmapNotFound<PaginationResult<List<PlainRoadmapDto>>>(userId);
        }
        var roadmapIds = dbUserRoadmapsResult.Data.Select(ur => ur.RoadmapId).ToList();
        var plainRoadmapsResult = await roadmapService.GetPlainRoadmapsByIds(roadmapIds, @params, ct, excludePrivate: false);
        if (!plainRoadmapsResult.IsSuccessful)
        {
            return ResultType.FailedToGetUserRoadmaps<PaginationResult<List<PlainRoadmapDto>>>(userId);
        }
        return plainRoadmapsResult;
    }

    public async Task<Result<bool>> RemoveRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmapResult = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (!dbUserRoadmapResult.HasData)
        {
            return ResultType.UserRoadmapNotFound<bool>(userId, roadmapId);
        }

        dbUserRoadmapResult.Data.IsActive = false;
        await userRoadmapsRepository.UpdateAsync(dbUserRoadmapResult.Data, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult.IsSuccessful)
        {
            return ResultType.FailedToRemoveRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }
}
