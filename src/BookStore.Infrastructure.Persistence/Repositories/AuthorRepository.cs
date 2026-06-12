using BookStore.Domain.Authors;
using BookStore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public class AuthorRepository(
  AppDbContext context
    ) : IAuthorRepository
{
  private readonly AppDbContext _context = context;

  public void Add(Author author)
  {
    _context.Authors.Add(author);
  }

  public void Update(Author author)
  {
    _context.Authors.Update(author);
  }

  public void Delete(Author author)
  {
    _context.Authors.Remove(author);
  }

  public Task<Author?> GetByIdAsync(AuthorId id, CancellationToken ct)
  {
    return _context.Authors
      .Include(a => a.Aliases)
      .FirstOrDefaultAsync(a => a.Id == id, ct);
  }
}
