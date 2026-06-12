namespace BookStore.Domain.Common;

public interface IDomainEvent
{
  string EventType { get; }

  DateTime OccurredOnUtc { get; }
}

public interface IAggregateRoot
{
  // IReadOnlyCollection<IDomainEvent> GetDomainEvents();
  IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

  void ClearDomainEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
  // Intentionally empty.
  // Signals that this class is the consistency boundary.
  // Only AggregateRoots are fetched/saved via Repositories.
  private readonly List<IDomainEvent> _domainEvents = [];

  public IReadOnlyCollection<IDomainEvent> DomainEvents =>
      _domainEvents.AsReadOnly();

  protected void RaiseDomainEvent(IDomainEvent domainEvent)
  {
    _domainEvents.Add(domainEvent);
  }

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }
}
