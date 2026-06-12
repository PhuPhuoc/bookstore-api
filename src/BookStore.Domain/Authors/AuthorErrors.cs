using ErrorOr;

namespace BookStore.Domain.Authors;

public static class AuthorErrors
{
  public static readonly Error NotFound = Error.NotFound(
      code: "Author.NotFound",
      description: "Author not found.");

  public static readonly Error InvalidLifeTime = Error.Validation(
      code: "Author.InvalidLifeTime",
      description: "Date of death cannot be earlier than date of birth.");

  public static readonly Error InvalidGender = Error.Validation(
          code: "Author.InvalidGender",
          description: "Gender must be one of: Male, Female, Other.");

  public static readonly Error DuplicateAlias = Error.Conflict(
      code: "Author.DuplicateAlias",
      description: "Author already has this alias.");

  public static readonly Error AliasNotFound = Error.NotFound(
      code: "Author.AliasNotFound",
      description: "Author alias not found.");
}
