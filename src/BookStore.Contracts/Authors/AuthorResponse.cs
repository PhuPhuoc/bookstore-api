namespace BookStore.Contracts.Authors;

public record AuthorResponse(Guid Id, string FirstName, string LastName, bool Gender, DateOnly DateOfBirth);
