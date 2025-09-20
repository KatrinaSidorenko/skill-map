using SkillMap.Business.Abstractions;
using SkillMap.Shared.Results;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SkillMap.Persistence;

internal class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly SkillMapDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(SkillMapDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<Result<bool>> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await  _dbSet.AddAsync(entity);
        return Result.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        if (entity == null)
        {
            return ResultType.NotFound<bool>($"{typeof(TEntity)} with id {id} not found");
        }

        _dbSet.Remove(entity);
        return Result.Success(true);
    }

    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        int? pageNum = null, int? count = null,
        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = _dbSet;
        try
        {
            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (pageNum.HasValue && count.HasValue)
                query = query.Skip((pageNum.Value - 1) * count.Value).Take(count.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TEntity>>("query_error", ex.Message);
        }

        var result = await query.ToListAsync(ct);
        return Result.Success<IEnumerable<TEntity>>(result);
    }

    public async Task<Result<TEntity>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        if (entity == null)
        {
            return ResultType.NotFound<TEntity>($"{typeof(TEntity)} with id {id} not found");
        }

        return Result.Success(entity);
    }

    public async Task<Result<TEntity>> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(filter, ct);
        if (entity == null)
        {
            return ResultType.NotFound<TEntity>($"{typeof(TEntity)} not found");
        }

        return Result.Success(entity);
    }

    public Task<Result<bool>> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.FromResult(Result.Success(true));
    }

    public async Task<Result<TEntity>> GetByPredicate(
               Expression<Func<TEntity, bool>> filter,
                      CancellationToken ct = default) 
    {
        var entity = await _dbSet.FirstOrDefaultAsync(filter, ct);
        if (entity == null)
        {
            return ResultType.NotFound<TEntity>($"{typeof(TEntity)} not found");
        }
        return Result.Success(entity);
    }

    public async Task<Result<bool>> IsUnique(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        var exists = await _dbSet.AnyAsync(filter, ct);
        return Result.Success(!exists);
    }

    public async Task<Result<bool>> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            var saved = await _context.SaveChangesAsync(ct);
            return Result.Success(saved > 0);
        }
        catch (Exception ex)
        {
            return ResultType.FailedToSave<bool>(ex.Message);
        }
    }
}
