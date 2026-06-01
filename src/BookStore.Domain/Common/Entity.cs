namespace BookStore.Domain.Common;

public interface IDomainEvent { }

public abstract class Entity<TId>
{
  public TId Id { get; protected set; } = default!;

  private readonly List<IDomainEvent> _domainEvents = [];
  public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public void RaiseDomainEvent(IDomainEvent domainEvent) =>
      _domainEvents.Add(domainEvent);

  public void ClearDomainEvents() =>
      _domainEvents.Clear();
}
