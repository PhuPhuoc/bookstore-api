using BookStore.Domain.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
  public void Configure(EntityTypeBuilder<Author> builder)
  {
    builder.ToTable("authors");

    builder.HasKey(a => a.Id);

    builder.Property(a => a.Id)
        .HasColumnName("id")
        .HasConversion(
            id => id.Value,
            value => new AuthorId(value))
        .ValueGeneratedNever();

    builder.Property(a => a.FirstName)
        .HasColumnName("first_name")
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(a => a.LastName)
        .HasColumnName("last_name")
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(a => a.IsActive)
        .HasColumnName("is_active")
        .IsRequired();

    builder.OwnsOne(a => a.Details, detailsBuilder =>
    {
      detailsBuilder.Property(d => d.Gender)
              .HasColumnName("gender")
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();

      detailsBuilder.Property(d => d.DateOfBirth)
              .HasColumnName("date_of_birth")
              .IsRequired();

      detailsBuilder.Property(d => d.Biography)
              .HasColumnName("biography")
              .HasMaxLength(2000);

      detailsBuilder.Property(d => d.Nationality)
              .HasColumnName("nationality")
              .HasMaxLength(100);

      detailsBuilder.Property(d => d.BirthPlace)
              .HasColumnName("birth_place")
              .HasMaxLength(200);

      detailsBuilder.Property(d => d.DateOfDeath)
              .HasColumnName("date_of_death");

      detailsBuilder.Property(d => d.PortraitImageUrl)
              .HasColumnName("portrait_image_url")
              .HasMaxLength(500);

      detailsBuilder.Property(d => d.OfficialWebsite)
              .HasColumnName("official_website")
              .HasMaxLength(500);
    });

    builder.OwnsMany(a => a.Aliases, aliasBuilder =>
    {
      aliasBuilder.ToTable("author_aliases");

      aliasBuilder.WithOwner()
              .HasForeignKey("author_id");

      aliasBuilder.HasKey(a => a.Id);

      aliasBuilder.Property(a => a.Id)
              .HasColumnName("id")
              .HasConversion(
                  id => id.Value,
                  value => new AuthorAliasId(value))
              .ValueGeneratedNever();

      aliasBuilder.Property(a => a.Name)
              .HasColumnName("name")
              .HasMaxLength(200)
              .IsRequired();

      aliasBuilder.Property(a => a.NormalizedName)
              .HasColumnName("normalized_name")
              .HasMaxLength(200)
              .IsRequired();

      aliasBuilder.HasIndex("author_id", nameof(AuthorAlias.NormalizedName))
              .IsUnique();
    });

    builder.Navigation(a => a.Details)
        .IsRequired();

    builder.Navigation(a => a.Aliases)
        .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}
