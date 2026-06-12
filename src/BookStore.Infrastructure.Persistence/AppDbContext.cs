using BookStore.Domain.Authors;
using BookStore.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Author> Authors { get; set; }

  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // auto pick up every IEntityTypeConfiguration in this assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    // base.OnModelCreating(modelBuilder);
  }
}

