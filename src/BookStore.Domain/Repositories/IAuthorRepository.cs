using BookStore.Domain.Authors;

namespace BookStore.Domain.Repositories;

public interface IAuthorRepository
{
  Task<Author?> GetByIdAsync(AuthorId id, CancellationToken ct = default);
  Task<List<Author>> ListAsync(CancellationToken ct = default);
  Task AddAsync(Author author, CancellationToken ct = default);
  Task UpdateAsync(Author author, CancellationToken ct = default);
  Task DeleteAsync(Author author, CancellationToken ct = default);
}
