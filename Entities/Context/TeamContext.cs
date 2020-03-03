using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Context
{
    public class TeamContext: DbContext
    {
        public TeamContext(DbContextOptions<TeamContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Sliver> Slivers { get; set; }
        public DbSet<UserSliver> UserSlivers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<UserBadge>().HasKey(p => new { p.UserId });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSliver>().HasKey(p => new { p.UserId });
            base.OnModelCreating(modelBuilder);
        }
    }


}
