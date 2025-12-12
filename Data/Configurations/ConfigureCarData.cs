using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snap.Core.Entities;

namespace Snap.Repository.Data.Configurations
{
    public class ConfigureCarData : IEntityTypeConfiguration<CarData>
    {
        public void Configure(EntityTypeBuilder<CarData> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CarPhoto).IsRequired(false);
            builder.Property(c => c.LicenseFront).IsRequired(false);
            builder.Property(c => c.LicenseBack).IsRequired(false);
            builder.Property(c => c.CarBrand).IsRequired();
            builder.Property(c => c.CarModel).IsRequired();
            builder.Property(c => c.CarColor).IsRequired();
            builder.Property(c => c.PlateNumber).IsRequired();
            builder.HasOne(c => c.Driver)
                .WithMany()
                .HasForeignKey(c => c.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
