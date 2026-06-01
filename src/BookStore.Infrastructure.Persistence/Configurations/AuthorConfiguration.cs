using BookStore.Domain.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
  public void Configure(EntityTypeBuilder<Author> builder)
  {
    builder.ToTable("authors");

    builder.HasKey(a => a.Id);

    // Map AuthorId (value object) → Guid column
    builder.Property(a => a.Id)
           .HasConversion(id => id.Value, value => new AuthorId(value))
           .HasColumnName("id");

    builder.Property(a => a.FirstName)
           .HasColumnName("first_name")
           .HasMaxLength(100)
           .IsRequired();

    builder.Property(a => a.LastName)
           .HasColumnName("last_name")
           .HasMaxLength(100)
           .IsRequired();

    builder.Property(a => a.Gender)
           .HasColumnName("gender")
           .IsRequired();

    builder.Property(a => a.DateOfBirth)
           .HasColumnName("date_of_birth")
           .IsRequired();
  }
}
