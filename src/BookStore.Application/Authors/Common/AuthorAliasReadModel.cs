namespace BookStore.Application.Authors.Common;

public sealed record AuthorAliasReadModel(
    Guid Id,
    string Name,
    string NormalizedName);
