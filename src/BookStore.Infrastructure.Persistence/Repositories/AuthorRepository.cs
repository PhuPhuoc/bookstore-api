using BookStore.Domain.Authors;
using BookStore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public class AuthorRepository(
  AppDbContext context
    ) : IAuthorRepository
{
  private readonly AppDbContext _context = context;

  public async Task AddAsync(Author author, CancellationToken ct)
  {
    await _context.Authors.AddAsync(author, ct);
    await _context.SaveChangesAsync(ct);
  }

  public async Task DeleteAsync(Author author, CancellationToken ct)
  {
    _context.Authors.Remove(author);
    await _context.SaveChangesAsync(ct);
  }

  public async Task<Author?> GetByIdAsync(AuthorId id, CancellationToken ct)
  {
    return await _context.Authors
        .FirstOrDefaultAsync(a => a.Id == id, ct);
  }

  public async Task<List<Author>> ListAsync(CancellationToken ct)
  {
    return await _context.Authors.ToListAsync(ct);
  }

  public async Task UpdateAsync(Author author, CancellationToken ct)
  {
    _context.Authors.Update(author);
    await _context.SaveChangesAsync(ct);
  }
}
