using BookStore.Domain.Authors;
using BookStore.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandHandler(
   IAuthorRepository repo
    ) : IRequestHandler<CreateAuthorCommand, ErrorOr<Author>>
{

  private readonly IAuthorRepository _repo = repo;

  public async Task<ErrorOr<Author>> Handle(CreateAuthorCommand cmd, CancellationToken ct)
  {
    var author = Author.Create(cmd.FirstName, cmd.LastName, cmd.Gender, cmd.DateOfBirth);
    await _repo.AddAsync(author, ct);
    return author;
  }
}
