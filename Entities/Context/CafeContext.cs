using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Context
{
    public class CafeContext: DbContext
    {
        public CafeContext(DbContextOptions<CafeContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<GeoData> GeoData { get; set; }
        public DbSet<MediaData> Media { get; set; }
        public DbSet<UserFacility> UserHasFacility { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Facility>()
                 .HasMany(c => c.UserFacilities)
                 .WithOne().HasForeignKey(p => p.FacilityId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Facility>()
                .HasMany(c => c.Geo)
                .WithOne().HasForeignKey(p => p.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Facility>()
                .HasMany(c => c.Media)
                .WithOne().HasForeignKey(p => p.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFacility>().HasKey(p => new { p.FacilityId, p.UserId });
        }
    }
}
