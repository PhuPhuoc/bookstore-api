using BookStore.Domain.Authors.Events;
using BookStore.Domain.Common;
using BookStore.Infrastructure.Persistence.Outbox.Payloads;

namespace BookStore.Infrastructure.Persistence.Outbox;

public static class OutboxPayloadFactory
{
  public static object CreatePayload(IDomainEvent domainEvent)
  {
    return domainEvent switch
    {
      AuthorCreatedDomainEvent authorCreated => new AuthorCreatedPayload(
          authorCreated.AuthorId.Value.ToString(),
          authorCreated.FirstName,
          authorCreated.LastName),

      _ => throw new InvalidOperationException(
          $"Unsupported domain event type: {domainEvent.GetType().Name}")
    };
  }
}
