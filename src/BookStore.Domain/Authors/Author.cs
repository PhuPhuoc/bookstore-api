using BookStore.Domain.Common;

namespace BookStore.Domain.Authors;

public class Author : AggregateRoot<AuthorId>
{
  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public bool Gender { get; private set; }
  public DateOnly DateOfBirth { get; private set; }

  private Author() { }   // EF needs this

  public static Author Create(string firstName, string lastName, bool gender, DateOnly dateOfBirth)
  {
    return new Author
    {
      Id = new AuthorId(Guid.NewGuid()),
      FirstName = firstName,
      LastName = lastName,
      Gender = gender,
      DateOfBirth = dateOfBirth
    };
  }

  public void Update(string firstName, string lastName, bool gender, DateOnly dateOfBirth)
  {
    FirstName = firstName;
    LastName = lastName;
    Gender = gender;
    DateOfBirth = dateOfBirth;
  }
}
