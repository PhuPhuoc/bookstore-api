using BookStore.Domain.Common;

namespace BookStore.Domain.Authors.Events;

public sealed record AuthorCreatedDomainEvent(
    AuthorId AuthorId,
    string FirstName,
    string LastName,
    DateTime OccurredOnUtc) : IDomainEvent
{
  public string EventType => AuthorEventTypes.AuthorCreated;
}
