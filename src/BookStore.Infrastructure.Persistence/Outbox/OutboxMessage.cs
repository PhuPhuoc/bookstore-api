namespace BookStore.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
  public Guid Id { get; private set; }

  public DateTime OccurredOnUtc { get; private set; }

  public string Type { get; private set; } = null!;

  public string Content { get; private set; } = null!;

  public DateTime? ProcessedOnUtc { get; private set; }

  public string? Error { get; private set; }

  private OutboxMessage()
  {
    // Required by EF Core.
  }

  public OutboxMessage(
      Guid id,
      DateTime occurredOnUtc,
      string type,
      string content)
  {
    Id = id;
    OccurredOnUtc = occurredOnUtc;
    Type = type;
    Content = content;
  }

  public void MarkAsProcessed(DateTime processedOnUtc)
  {
    ProcessedOnUtc = processedOnUtc;
    Error = null;
  }

  public void MarkAsFailed(string error)
  {
    Error = error;
  }
}
