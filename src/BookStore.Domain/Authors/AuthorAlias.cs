using BookStore.Domain.Common;

namespace BookStore.Domain.Authors;

public sealed class AuthorAlias : Entity<AuthorAliasId>
{
  public string Name { get; private set; } = null!;
  public string NormalizedName { get; private set; } = null!;

  private AuthorAlias()
  {
    // Required by EF Core.
  }

  private AuthorAlias(AuthorAliasId id, string name)
  {
    Id = id;
    Name = name.Trim();
    NormalizedName = Normalize(name);
  }

  public static AuthorAlias Create(string name)
  {
    return new AuthorAlias(
        AuthorAliasId.New(),
        name);
  }

  public void Rename(string name)
  {
    Name = name.Trim();
    NormalizedName = Normalize(name);
  }

  private static string Normalize(string name)
  {
    return name.Trim().ToUpperInvariant();
  }
}
