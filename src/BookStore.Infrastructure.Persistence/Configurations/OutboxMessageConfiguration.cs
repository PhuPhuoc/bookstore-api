using BookStore.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
  public void Configure(EntityTypeBuilder<OutboxMessage> builder)
  {
    builder.ToTable("outbox_messages");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
        .HasColumnName("id");

    builder.Property(x => x.OccurredOnUtc)
        .HasColumnName("occurred_on_utc")
        .IsRequired();

    builder.Property(x => x.Type)
        .HasColumnName("type")
        .HasMaxLength(500)
        .IsRequired();

    builder.Property(x => x.Content)
        .HasColumnName("content")
        .HasColumnType("jsonb")
        .IsRequired();

    builder.Property(x => x.ProcessedOnUtc)
        .HasColumnName("processed_on_utc");

    builder.Property(x => x.Error)
        .HasColumnName("error");

    builder.HasIndex(x => x.ProcessedOnUtc);
  }
}
