using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountCodes.Persistence.Entities.Configurations;

internal class DiscountCodeConfiguration : IEntityTypeConfiguration<DiscountCode>
{
    public void Configure(EntityTypeBuilder<DiscountCode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
         .ValueGeneratedOnAdd()
         .HasAnnotation("Sqlite:Autoincrement", true); ;

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.IsUsed).IsRequired();

        builder.Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}