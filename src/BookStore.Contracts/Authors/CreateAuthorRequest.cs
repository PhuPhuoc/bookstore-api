namespace BookStore.Contracts.Authors;

public record CreateAuthorRequest(
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
    IReadOnlyCollection<string>? Aliases
    );
