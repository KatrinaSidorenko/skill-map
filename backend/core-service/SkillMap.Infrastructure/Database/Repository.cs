using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using SkillMap.Business.Abstractions;
using SkillMap.Persistence;

namespace SkillMap.Infrastructure;

// todo: as no tracking question??
internal class Repository<TEntity> : IRepository<TEntity>
 where TEntity : class
{
    protected readonly SkillMapDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(SkillMapDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<bool> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return true;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        if (entity == null)
        {
            return false;
        }

        _dbSet.Remove(entity);
        return true;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        int? pageNum = null, int? count = null,
        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy.Compile()(query);

        if (pageNum.HasValue && count.HasValue)
            query = query.Skip((pageNum.Value - 1) * count.Value).Take(count.Value);

        if (count.HasValue && !pageNum.HasValue)
            query = query.Take(count.Value);

        var result = await query.AsNoTracking().ToListAsync(ct);
        return result;
    }

    public async Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        return entity;
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(filter, ct);
        return entity;
    }

    public Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.FromResult(true);
    }

    public async Task<TEntity?> GetByPredicate(
    Expression<Func<TEntity, bool>> filter,
    CancellationToken ct = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(filter, ct);
        return entity;
    }

    public async Task<bool> IsUnique(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        var exists = await _dbSet.AnyAsync(filter, ct);
        return !exists;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
    {
        var saved = await _context.SaveChangesAsync(ct);
        return saved > 0;
    }
}