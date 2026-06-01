using BookStore.Domain.Authors;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<Author> Authors { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // auto pick up every IEntityTypeConfiguration in this assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}

