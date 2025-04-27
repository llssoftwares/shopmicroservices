namespace Catalog.Admin.API.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder
            .Property(x => x.Price)
            .HasColumnType("decimal")
            .HasPrecision(18, 2);
    }
}