using BookStore.Domain.Authors;
using ErrorOr;
using MediatR;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(string FirstName, string LastName, bool Gender, DateOnly DateOfBirth) : IRequest<ErrorOr<Author>>;
