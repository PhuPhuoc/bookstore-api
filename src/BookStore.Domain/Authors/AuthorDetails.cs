namespace BookStore.Domain.Authors;

public sealed record AuthorDetails
{
  public Gender Gender { get; private set; }
  public DateOnly DateOfBirth { get; private set; }
  public string? Biography { get; private set; }
  public string? Nationality { get; private set; }
  public string? BirthPlace { get; private set; }
  public DateOnly? DateOfDeath { get; private set; }
  public string? PortraitImageUrl { get; private set; }
  public string? OfficialWebsite { get; private set; }

  private AuthorDetails()
  {
    // Required by EF Core.
  }

  private AuthorDetails(
      Gender gender,
      DateOnly dateOfBirth,
      string? biography,
      string? nationality,
      string? birthPlace,
      DateOnly? dateOfDeath,
      string? portraitImageUrl,
      string? officialWebsite)
  {
    Gender = gender;
    DateOfBirth = dateOfBirth;
    Biography = Normalize(biography);
    Nationality = Normalize(nationality);
    BirthPlace = Normalize(birthPlace);
    DateOfDeath = dateOfDeath;
    PortraitImageUrl = Normalize(portraitImageUrl);
    OfficialWebsite = Normalize(officialWebsite);
  }

  public static AuthorDetails Create(
      Gender gender,
      DateOnly dateOfBirth,
      string? biography = null,
      string? nationality = null,
      string? birthPlace = null,
      DateOnly? dateOfDeath = null,
      string? portraitImageUrl = null,
      string? officialWebsite = null)
  {
    return new AuthorDetails(
        gender,
        dateOfBirth,
        biography,
        nationality,
        birthPlace,
        dateOfDeath,
        portraitImageUrl,
        officialWebsite);
  }

  public static AuthorDetails Empty => new(
      gender: Gender.Unknown,
      dateOfBirth: default,
      biography: null,
      nationality: null,
      birthPlace: null,
      dateOfDeath: null,
      portraitImageUrl: null,
      officialWebsite: null);

  private static string? Normalize(string? value)
  {
    return string.IsNullOrWhiteSpace(value)
        ? null
        : value.Trim();
  }
}
