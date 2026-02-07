using SkillMap.Core.Entities.UserRoadmapTest;

namespace SkillMap.Business.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}