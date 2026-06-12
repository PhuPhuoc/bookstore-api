using BookStore.Domain.Authors;
using BookStore.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public sealed class CreateAuthorCommandHandler(
   IAuthorRepository repo
    ) : IRequestHandler<CreateAuthorCommand, ErrorOr<Author>>
{
  private readonly IAuthorRepository _authorRepository = repo;

  public async Task<ErrorOr<Author>> Handle(CreateAuthorCommand cmd, CancellationToken ct)
  {
    var gender = Enum.Parse<Gender>(
        cmd.Gender,
        ignoreCase: true);

    var author = Author.Create(
              cmd.FirstName,
              cmd.LastName,
              gender,
              cmd.DateOfBirth,
              cmd.Biography,
              cmd.Nationality,
              cmd.BirthPlace,
              cmd.DateOfDeath,
              cmd.PortraitImageUrl,
              cmd.OfficialWebsite);

    if (author.IsError)
    {
      return author.Errors;
    }

    _authorRepository.Add(author.Value);
    return author;
  }
}
