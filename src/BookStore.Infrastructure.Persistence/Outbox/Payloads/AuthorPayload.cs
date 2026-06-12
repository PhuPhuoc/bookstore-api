namespace BookStore.Infrastructure.Persistence.Outbox.Payloads;

public sealed record AuthorCreatedPayload(
    string AuthorId,
    string FirstName,
    string LastName);
