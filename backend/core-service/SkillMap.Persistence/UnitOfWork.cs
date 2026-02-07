using SkillMap.Business.Abstractions;

namespace SkillMap.Persistence;

public class UnitOfWork(SkillMapDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    public void Dispose() => context.Dispose();

    public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        => new Repository<TEntity>(context);
}