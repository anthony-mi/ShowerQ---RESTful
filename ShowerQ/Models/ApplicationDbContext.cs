using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowerQ.Models.Entities;

namespace ShowerQ.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
               .HasMany(c => c.Universities)
               .WithOne(u => u.City)
               .HasForeignKey(u => u.CityId);

            modelBuilder.Entity<Dormitory>()
                .HasOne(d => d.University)
                .WithMany(u => u.Dormitories)
                .HasForeignKey(d => d.UniversityId);

            modelBuilder.Entity<Dormitory>()
                .HasMany(d => d.Tenants);

            modelBuilder.Entity<Dormitory>()
                .HasMany(d => d.Administrators);

            modelBuilder.Entity<Dormitory>()
                .HasOne(d => d.CurrentSchedule);

            modelBuilder.Entity<Schedule>()
               .HasMany(s => s.Intervals)
               .WithOne(i => i.Schedule)
               .HasForeignKey(i => i.ScheduleId);

            modelBuilder.Entity<Interval>()
               .HasOne(i => i.Schedule)
               .WithMany(s => s.Intervals)
               .HasForeignKey(i => i.ScheduleId);

            modelBuilder.Entity<University>()
                .HasOne(u => u.City)
                .WithMany(c => c.Universities)
                .HasForeignKey(u => u.CityId);

            modelBuilder.Entity<University>()
                .HasMany(u => u.Dormitories)
                .WithOne(d => d.University)
                .HasForeignKey(d => d.UniversityId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Dormitory> Dormitories { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Interval> Intervals { get; set; }
        public DbSet<University> Universities { get; set; }
        public new DbSet<IdentityUser> Users { get; set; }
    }
}
