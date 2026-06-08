using BookStore.Application.Common.Messaging;
using BookStore.Domain.Authors;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(string FirstName, string LastName, bool Gender, DateOnly DateOfBirth) : ICommand<Author>;
