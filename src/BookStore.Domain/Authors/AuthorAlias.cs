using BookStore.Domain.Common;
using ErrorOr;

namespace BookStore.Domain.Authors;

public sealed class AuthorAlias : Entity<AuthorAliasId>
{
  public string Name { get; private set; } = null!;
  public string NormalizedName { get; private set; } = null!;

  private AuthorAlias()
  {
    // Required by EF Core.
  }

  private AuthorAlias(AuthorAliasId id, string name, string normalizedName)
  {
    Id = id;
    Name = name.Trim();
    NormalizedName = Normalize(normalizedName);
  }

  public static ErrorOr<AuthorAlias> Create(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return AuthorErrors.InvalidAliasName;
    }

    return new AuthorAlias(
        AuthorAliasId.New(),
        name.Trim(),
        name.Trim().ToUpperInvariant());
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
