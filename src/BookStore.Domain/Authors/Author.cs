using BookStore.Domain.Authors.Events;
using BookStore.Domain.Common;
using ErrorOr;

namespace BookStore.Domain.Authors;

public sealed class Author : AggregateRoot<AuthorId>
{
  public string FirstName { get; private set; } = null!;
  public string LastName { get; private set; } = null!;

  public AuthorDetails Details { get; private set; } = AuthorDetails.Empty;

  private readonly List<AuthorAlias> _aliases = [];
  public IReadOnlyCollection<AuthorAlias> Aliases => _aliases.AsReadOnly();

  public bool IsActive { get; private set; } = true;

  private Author()
  {
    // Required by EF Core.
  }

  private Author(
      AuthorId id,
      string firstName,
      string lastName,
      AuthorDetails details)
  {
    Id = id;
    FirstName = firstName.Trim();
    LastName = lastName.Trim();
    Details = details;
  }

  public static ErrorOr<Author> Create(
      string firstName,
      string lastName,
      Gender gender,
      DateOnly dateOfBirth,
      string? biography = null,
      string? nationality = null,
      string? birthPlace = null,
      DateOnly? dateOfDeath = null,
      string? portraitImageUrl = null,
      string? officialWebsite = null)
  {
    if (dateOfDeath is not null && dateOfDeath < dateOfBirth)
    {
      return AuthorErrors.InvalidLifeTime;
    }

    var details = AuthorDetails.Create(
        gender,
        dateOfBirth,
        biography,
        nationality,
        birthPlace,
        dateOfDeath,
        portraitImageUrl,
        officialWebsite);

    var author = new Author(
        AuthorId.New(),
        firstName,
        lastName,
        details);

    author.RaiseDomainEvent(new AuthorCreatedDomainEvent(
        author.Id,
        author.FirstName,
        author.LastName,
        DateTime.UtcNow));

    return author;
  }

  public ErrorOr<Success> Update(
      string firstName,
      string lastName,
      Gender gender,
      DateOnly dateOfBirth,
      string? biography,
      string? nationality,
      string? birthPlace,
      DateOnly? dateOfDeath,
      string? portraitImageUrl,
      string? officialWebsite)
  {
    if (dateOfDeath is not null && dateOfDeath < dateOfBirth)
    {
      return AuthorErrors.InvalidLifeTime;
    }

    FirstName = firstName.Trim();
    LastName = lastName.Trim();
    Details = AuthorDetails.Create(
        gender,
        dateOfBirth,
        biography,
        nationality,
        birthPlace,
        dateOfDeath,
        portraitImageUrl,
        officialWebsite);

    return Result.Success;
  }

  public ErrorOr<Success> ReplaceDetails(AuthorDetails details)
  {
    if (details.DateOfDeath is not null && details.DateOfDeath < details.DateOfBirth)
    {
      return AuthorErrors.InvalidLifeTime;
    }

    Details = details;

    return Result.Success;
  }

  public ErrorOr<Success> AddAlias(string name)
  {
    var alias = AuthorAlias.Create(name);

    var duplicated = _aliases.Any(x =>
        x.NormalizedName == alias.NormalizedName);

    if (duplicated)
    {
      return AuthorErrors.DuplicateAlias;
    }

    _aliases.Add(alias);

    return Result.Success;
  }

  public ErrorOr<Success> RenameAlias(
      AuthorAliasId aliasId,
      string name)
  {
    var alias = _aliases.FirstOrDefault(x => x.Id == aliasId);

    if (alias is null)
    {
      return AuthorErrors.AliasNotFound;
    }

    var normalizedName = name.Trim().ToUpperInvariant();

    var duplicated = _aliases.Any(x =>
        x.Id != aliasId &&
        x.NormalizedName == normalizedName);

    if (duplicated)
    {
      return AuthorErrors.DuplicateAlias;
    }

    alias.Rename(name);

    return Result.Success;
  }

  public ErrorOr<Success> RemoveAlias(AuthorAliasId aliasId)
  {
    var alias = _aliases.FirstOrDefault(x => x.Id == aliasId);

    if (alias is null)
    {
      return AuthorErrors.AliasNotFound;
    }

    _aliases.Remove(alias);

    return Result.Success;
  }

  public void Activate()
  {
    IsActive = true;
  }

  public void Deactivate()
  {
    IsActive = false;
  }
}
