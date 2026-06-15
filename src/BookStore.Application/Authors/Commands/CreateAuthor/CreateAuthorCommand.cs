using BookStore.Application.Common.Messaging;
using BookStore.Domain.Authors;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(
    string FirstName,
    string LastName,
    string Gender,
    DateOnly DateOfBirth,
    string? Biography,
    string? Nationality,
    string? BirthPlace,
    DateOnly? DateOfDeath,
    string? PortraitImageUrl,
    string? OfficialWebsite,
    IReadOnlyCollection<string>? Aliases
    ) : ICommand<Author>;
