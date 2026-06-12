using BookStore.Application.Common.Messaging;
using BookStore.Domain.Authors;

namespace BookStore.Application.Authors.Commands.UpdateAuthor;

public record UpdateAuthorCommand(
    AuthorId Id,
    string FirstName,
    string LastName,
    Gender Gender,
    DateOnly DateOfBirth,
    string? Biography,
    string? Nationality,
    string? BirthPlace,
    DateOnly? DateOfDeath,
    string? PortraitImageUrl,
    string? OfficialWebsite
    ) : ICommand<Author>;
