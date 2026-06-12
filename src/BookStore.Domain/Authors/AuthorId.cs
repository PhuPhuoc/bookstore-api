namespace BookStore.Domain.Authors;

public readonly record struct AuthorId(Guid Value)
{
  public static AuthorId New() => new(Guid.NewGuid());
}
