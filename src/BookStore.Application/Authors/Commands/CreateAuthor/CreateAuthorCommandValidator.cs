using BookStore.Domain.Authors;
using FluentValidation;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
  public CreateAuthorCommandValidator()
  {
    RuleFor(x => x.FirstName)
        .NotEmpty()
        .MaximumLength(30);

    RuleFor(x => x.LastName)
        .NotEmpty()
        .MaximumLength(30);

    RuleFor(x => x.Gender)
        .NotEmpty()
        .Must(BeValidGender)
        .WithMessage("'Gender' must be one of: Male, Female, Other.");

    RuleFor(x => x.DateOfBirth)
        .NotEmpty();

    RuleFor(x => x.DateOfDeath)
        .GreaterThanOrEqualTo(x => x.DateOfBirth)
        .When(x => x.DateOfDeath is not null)
        .WithMessage("'DateOfDeath' cannot be earlier than 'DateOfBirth'.");
  }

  private static bool BeValidGender(string gender)
  {
    return Enum.TryParse<Gender>(
        gender,
        ignoreCase: true,
        out var parsedGender)
        && parsedGender != Gender.Unknown;
  }
}
