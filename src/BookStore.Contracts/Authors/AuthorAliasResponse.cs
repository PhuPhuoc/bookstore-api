namespace BookStore.Contracts.Authors;

public sealed record AuthorAliasResponse(
    Guid Id,
    string Name);
