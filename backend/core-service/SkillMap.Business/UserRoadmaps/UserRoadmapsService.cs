using SkillMap.Business.Abstractions;
using SkillMap.Business.UserRoadmaps.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Business.UserRoadmaps;

public class UserRoadmapsService(IRepository<UserRoadmap> userRoadmapsRepository) : IUserRoadmapsService
{
    public async Task<Result<bool>> AddRoadmap(long userId, string roadmapId, CancellationToken ct)
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
            IsActive = true
        };

        await userRoadmapsRepository.AddAsync(userRoadmap, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult.IsSuccessful)
        {
            return ResultTypes.FailedToAddRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }

    public async Task<Result<UserRoadmapDto>> GetUserRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmapResult = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (!dbUserRoadmapResult.HasData)
        {
            return ResultTypes.UserRoadmapNotFound<UserRoadmapDto>(userId, roadmapId);
        }

        var userRoadmap = new UserRoadmapDto
        {
            Id = dbUserRoadmapResult.Data.Id,
            UserId = dbUserRoadmapResult.Data.UserId,
            RoadmapId = dbUserRoadmapResult.Data.RoadmapId,
            IsActive = dbUserRoadmapResult.Data.IsActive
        };

        return Result.Success(userRoadmap);
    }

    public async Task<Result<List<UserRoadmapDto>>> GetUserRoadmaps(long userId, CancellationToken ct)
    {
        var dbUserRoadmapsResult = await userRoadmapsRepository.GetAllAsync(ur => ur.UserId == userId && ur.IsActive, ct: ct);
        if (!dbUserRoadmapsResult.HasData)
        {
            return ResultTypes.UserRoadmapNotFound<List<UserRoadmapDto>>(userId);
        }

        var userRoadmaps = dbUserRoadmapsResult.Data.Select(ur => new UserRoadmapDto
        {
            Id = ur.Id,
            UserId = ur.UserId,
            RoadmapId = ur.RoadmapId,
            IsActive = ur.IsActive
        }).ToList();

        return Result.Success(userRoadmaps);
    }

    public async Task<Result<bool>> RemoveRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var dbUserRoadmapResult = await userRoadmapsRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoadmapId == roadmapId, ct: ct);
        if (!dbUserRoadmapResult.HasData)
        {
            return ResultTypes.UserRoadmapNotFound<bool>(userId, roadmapId);
        }

        dbUserRoadmapResult.Data.IsActive = false;
        await userRoadmapsRepository.UpdateAsync(dbUserRoadmapResult.Data, ct);
        var saveResult = await userRoadmapsRepository.SaveChangesAsync(ct);
        if (!saveResult.IsSuccessful)
        {
            return ResultTypes.FailedToRemoveRoadmap<bool>(userId, roadmapId);
        }

        return Result.Success(true);
    }


}
