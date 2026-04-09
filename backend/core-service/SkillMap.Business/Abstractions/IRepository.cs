using System.Linq.Expressions;

using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> AddAsync(TEntity entity, CancellationToken ct = default);

    Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default);

    Task<IEnumerable<TEntity>> GetAllAsync(
         Expression<Func<TEntity, bool>>? filter = null,
          Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
          int? pageNum = null, int? count = null,
          CancellationToken ct = default);

    Task<bool> DeleteAsync(long id, CancellationToken ct = default);

    Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default);

    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken ct = default);

    Task<bool> IsUnique(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
    Task<TEntity?> GetByPredicate(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
}