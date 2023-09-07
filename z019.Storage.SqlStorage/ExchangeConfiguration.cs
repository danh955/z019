namespace z019.Storage.SqlStorage;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
{
    public void Configure(EntityTypeBuilder<Exchange> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasColumnName("Name");
        builder.Property(e => e.Code).HasColumnName("Code");
        builder.Property(e => e.OperatingMIC).HasColumnName("OperatingMIC");
        builder.Property(e => e.Country).HasColumnName("Country");
        builder.Property(e => e.Currency).HasColumnName("Currency");
        builder.Property(e => e.CountryISO2).HasColumnName("CountryISO2");
        builder.Property(e => e.CountryISO3).HasColumnName("CountryISO3");

        builder.HasIndex(e => e.Code)
            .IsUnique();
    }
}