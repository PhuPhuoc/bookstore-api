namespace BookStore.Application.Authors.Common;

public interface IAuthorReadRepository
{
  Task<AuthorReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default);

  // Task<IReadOnlyList<AuthorListItemReadModel>> ListAsync(CancellationToken ct = default);
}
