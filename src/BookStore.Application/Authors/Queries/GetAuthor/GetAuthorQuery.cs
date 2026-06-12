using BookStore.Application.Authors.Common;
using BookStore.Application.Common.Messaging;

namespace BookStore.Application.Authors.Queries.GetAuthor;

public record GetAuthorQuery(Guid Id) : IQuery<AuthorReadModel>;
