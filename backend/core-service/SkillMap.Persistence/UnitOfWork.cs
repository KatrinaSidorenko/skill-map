using SkillMap.Business.Abstractions;

namespace SkillMap.Persistence;

public class UnitOfWork(SkillMapDbContext context) : IUnitOfWork
{
    public IRepository<TEntity> CreateRepository<TEntity>(TEntity entityType)
        where TEntity : class
        => new Repository<TEntity>(context);
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    public void Dispose() => context.Dispose();
}

