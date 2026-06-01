namespace BookStore.Contracts.Authors;

public record UpdateAuthorRequest(string FirstName, string LastName, bool Gender, DateOnly DateOfBirth);
