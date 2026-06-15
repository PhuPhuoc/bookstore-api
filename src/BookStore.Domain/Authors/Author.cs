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
      string? officialWebsite = null,
      IReadOnlyCollection<string>? aliases = null)
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


    if (aliases is not null)
    {
      foreach (var aliasName in aliases)
      {
        var addAliasResult = author.AddAlias(aliasName);

        if (addAliasResult.IsError)
        {
          return addAliasResult.Errors;
        }
      }
    }

    author.RaiseDomainEvent(new AuthorCreatedDomainEvent(
        author.Id,
        author.FirstName,
        author.LastName,
        author.Details.Gender.ToString(),
        author.Details.DateOfBirth,
        author.Details.Biography,
        author.Details.Nationality,
        author.Details.BirthPlace,
        author.Details.DateOfDeath,
        author.Details.PortraitImageUrl,
        author.Details.OfficialWebsite,
        [.. author.Aliases.Select(a => a.NormalizedName)],
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
        x.NormalizedName == alias.Value.NormalizedName);

    if (duplicated)
    {
      return AuthorErrors.DuplicateAlias;
    }

    _aliases.Add(alias.Value);

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
