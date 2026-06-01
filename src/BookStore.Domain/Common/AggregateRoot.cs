namespace BookStore.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
{
  // Intentionally empty.
  // Signals that this class is the consistency boundary.
  // Only AggregateRoots are fetched/saved via Repositories.
}
