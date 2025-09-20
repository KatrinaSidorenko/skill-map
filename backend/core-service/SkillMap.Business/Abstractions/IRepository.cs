using SkillMap.Shared.Results;
using System.Linq.Expressions;

namespace SkillMap.Business.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<bool>> AddAsync(TEntity entity, CancellationToken ct = default);

    Task<Result<TEntity>> GetByIdAsync(long id, CancellationToken ct = default);

    Task<Result<IEnumerable<TEntity>>> GetAllAsync(
         Expression<Func<TEntity, bool>>? filter = null,
          Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
          int? pageNum = null, int? count = null,
          CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(long id, CancellationToken ct = default);

    Task<Result<bool>> UpdateAsync(TEntity entity, CancellationToken ct = default);

    Task<Result<TEntity>> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken ct = default);

    Task<Result<bool>> IsUnique(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
    Task<Result<bool>> SaveChangesAsync(CancellationToken ct = default);
    Task<Result<TEntity>> GetByPredicate(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
}
