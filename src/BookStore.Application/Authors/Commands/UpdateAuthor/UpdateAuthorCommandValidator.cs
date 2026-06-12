using BookStore.Domain.Authors;
using FluentValidation;

namespace BookStore.Application.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
  public UpdateAuthorCommandValidator()
  {
    RuleFor(x => x.Id.Value)
        .NotEmpty()
        .WithMessage("'AuthorId' must not be empty.");

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

    RuleFor(x => x.Biography)
        .MaximumLength(2000)
        .When(x => x.Biography is not null);

    RuleFor(x => x.Nationality)
        .MaximumLength(100)
        .When(x => x.Nationality is not null);

    RuleFor(x => x.BirthPlace)
        .MaximumLength(200)
        .When(x => x.BirthPlace is not null);

    RuleFor(x => x.PortraitImageUrl)
        .MaximumLength(500)
        .When(x => x.PortraitImageUrl is not null);

    RuleFor(x => x.OfficialWebsite)
        .MaximumLength(500)
        .When(x => x.OfficialWebsite is not null);
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
