using BookStore.Application.Common.Interfaces.Persistence;

namespace BookStore.Infrastructure.Persistence.Common;

public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
  public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return dbContext.SaveChangesAsync(cancellationToken);
  }
}
