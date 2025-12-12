using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Snap.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Repository.Data
{
    public class SnapDbContext: IdentityDbContext<User>
    {
        public SnapDbContext(DbContextOptions<SnapDbContext>options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // Configure LatLng as owned type for Order
            modelBuilder.Entity<Order>().OwnsOne(o => o.FromLatLng);
            modelBuilder.Entity<Order>().OwnsOne(o => o.ToLatLng);
            base.OnModelCreating(modelBuilder);
        }

        // DbSet properties for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<CarData> CarDatas { get; set; }
        public DbSet<Charge> Charges { get; set; } 
        public DbSet<TripsHistory> TripsHistories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
    }
}
