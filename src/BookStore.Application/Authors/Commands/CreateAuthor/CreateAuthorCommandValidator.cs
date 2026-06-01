using FluentValidation;

namespace BookStore.Application.Authors.Commands.CreateAuthor;


public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
  public CreateAuthorCommandValidator()
  {
    RuleFor(x => x.FirstName).NotEmpty().MaximumLength(30);
    RuleFor(x => x.LastName).NotEmpty().MaximumLength(30);
    RuleFor(x => x.Gender).NotEmpty();
    RuleFor(x => x.DateOfBirth).NotEmpty();
  }
}
