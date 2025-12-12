using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snap.Core.Entities;

namespace Snap.Repository.Data.Configurations
{
    public class ConfigureDriver : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.DriverFullname).IsRequired();
            builder.Property(d => d.NationalId).IsRequired();
            builder.Property(d => d.LicenseNumber).IsRequired();
            builder.Property(d => d.Email).IsRequired();
            builder.Property(d => d.Password).IsRequired();
            builder.Property(d => d.LicenseExpiryDate).IsRequired();
            builder.HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
