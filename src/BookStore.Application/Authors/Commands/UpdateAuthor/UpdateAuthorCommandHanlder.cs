using BookStore.Domain.Authors;
using BookStore.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace BookStore.Application.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandHandler(
   IAuthorRepository repo
    ) : IRequestHandler<UpdateAuthorCommand, ErrorOr<Author>>
{
  private readonly IAuthorRepository _authorRepository = repo;

  public async Task<ErrorOr<Author>> Handle(UpdateAuthorCommand cmd, CancellationToken ct)
  {
    var author = await _authorRepository.GetByIdAsync(cmd.Id, ct);

    if (author is null)
    {
      return AuthorErrors.NotFound;
    }

    author.Update(
      cmd.FirstName,
      cmd.LastName,
      cmd.Gender,
      cmd.DateOfBirth,
      cmd.Biography,
      cmd.Nationality,
      cmd.BirthPlace,
      cmd.DateOfDeath,
      cmd.PortraitImageUrl,
      cmd.OfficialWebsite);

    _authorRepository.Update(author);
    return author;
  }
}
