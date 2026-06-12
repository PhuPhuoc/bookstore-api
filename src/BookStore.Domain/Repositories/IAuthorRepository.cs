using BookStore.Domain.Authors;

namespace BookStore.Domain.Repositories;

public interface IAuthorRepository
{
  void Add(Author author);

  void Update(Author author);

  void Delete(Author author);

  Task<Author?> GetByIdAsync(AuthorId id, CancellationToken ct = default);
}
