namespace BookStore.Infrastructure.Persistence.Outbox.Payloads;

public sealed record AuthorCreatedPayload(
    string Id,
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
    IReadOnlyCollection<string>? Aliases);
