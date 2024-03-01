using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Models;
using System.Diagnostics;
using System.Net;

namespace ParkingHUB.Data
{
    public class DataContext: IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ParkingVehicle> ParkingVehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ParkingVehicle>()
                .HasKey(ph => new { ph.ParkingId, ph.VehicleId });
            modelBuilder.Entity<ParkingVehicle>()
                .HasOne(p => p.Parking)
                .WithMany(ph => ph.ParkingVehicles)
                .HasForeignKey(p => p.ParkingId);
            modelBuilder.Entity<ParkingVehicle>()
                .HasOne(v => v.Vehicle)
                .WithMany(ph => ph.ParkingVehicles)
                .HasForeignKey(v => v.VehicleId);
        }
    }
}
