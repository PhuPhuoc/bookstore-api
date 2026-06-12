namespace BookStore.Domain.Authors;

public readonly record struct AuthorAliasId(Guid Value)
{
  public static AuthorAliasId New() => new(Guid.NewGuid());
}
