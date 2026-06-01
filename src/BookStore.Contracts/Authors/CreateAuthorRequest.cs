namespace BookStore.Contracts.Authors;

public record CreateAuthorRequest(string FirstName, string LastName, bool Gender, DateOnly DateOfBirth);
