// using BookStore.Application.Authors.Common;
// using BookStore.Domain.Authors;
// using ErrorOr;
// using MediatR;

namespace BookStore.Application.Authors.Queries.GetAuthor;

// public class GetAuthorQueryHandler(
//     IAuthorReadRepository authorReadRepository)
//     : IRequestHandler<GetAuthorQuery, ErrorOr<AuthorReadModel>>
// {
//   public async Task<ErrorOr<AuthorReadModel>> Handle(
//       GetAuthorQuery query,
//       CancellationToken cancellationToken)
//   {
//     var author = await authorReadRepository.GetByIdAsync(
//         query.Id,
//         cancellationToken);
//
//     if (author is null)
//     {
//       return AuthorErrors.NotFound;
//     }
//
//     return author;
//   }
// }
