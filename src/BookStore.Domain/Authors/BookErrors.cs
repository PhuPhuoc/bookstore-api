using ErrorOr;
namespace BookStore.Domain.Authors;

public static class AuthorErrors
{
  public static readonly Error NotFound = Error.NotFound("Author.NotFound", "Author not found.");
}
