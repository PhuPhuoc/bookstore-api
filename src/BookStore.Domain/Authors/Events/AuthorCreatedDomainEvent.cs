using BookStore.Domain.Common;

namespace BookStore.Domain.Authors.Events;

public sealed record AuthorCreatedDomainEvent(
    AuthorId AuthorId,
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
    IReadOnlyCollection<string>? Aliases,
    DateTime OccurredOnUtc) : IDomainEvent
{
  public string EventType => AuthorEventTypes.AuthorCreated;
}
