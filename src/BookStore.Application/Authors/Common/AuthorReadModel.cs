namespace BookStore.Application.Authors.Common;

public sealed record AuthorReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Gender,
    DateOnly DateOfBirth,
    string? Biography,
    string? Nationality,
    string? BirthPlace,
    DateOnly? DateOfDeath,
    string? PortraitImageUrl,
    string? OfficialWebsite,
    bool IsActive,
    IReadOnlyList<AuthorAliasReadModel> Aliases);
