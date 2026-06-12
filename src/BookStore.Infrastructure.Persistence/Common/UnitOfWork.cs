using System.Text.Json;
using BookStore.Application.Common.Interfaces.Persistence;
using BookStore.Domain.Common;
using BookStore.Infrastructure.Persistence.Outbox;

namespace BookStore.Infrastructure.Persistence.Common;

public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
  public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    AddDomainEventsAsOutboxMessages();
    return dbContext.SaveChangesAsync(cancellationToken);
  }

  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    WriteIndented = false,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  private void AddDomainEventsAsOutboxMessages()
  {
    var aggregateRoots = dbContext.ChangeTracker
        .Entries<IAggregateRoot>()
        .Where(entry => entry.Entity.DomainEvents.Count > 0)
        .Select(entry => entry.Entity)
        .ToList();

    var domainEvents = aggregateRoots
        .SelectMany(aggregateRoot => aggregateRoot.DomainEvents)
        .ToList();

    foreach (var aggregateRoot in aggregateRoots)
    {
      aggregateRoot.ClearDomainEvents();
    }

    var outboxMessages = domainEvents
      .Select(domainEvent =>
      {
        var payload = OutboxPayloadFactory.CreatePayload(domainEvent);

        return new OutboxMessage(
          Guid.NewGuid(),
          domainEvent.OccurredOnUtc,
          domainEvent.EventType,
          JsonSerializer.Serialize(
            payload,
            payload.GetType(),
            JsonSerializerOptions));
      })
      .ToList();

    dbContext.OutboxMessages.AddRange(outboxMessages);
  }
}
