using Microsoft.EntityFrameworkCore;
using ShowerQ.Models.Entities;
using ShowerQ.Models.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models
{
    public class ApplicationDbContext : DbContext
    {
        #region Users
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<DormitoryAdministrator> DormitoryAdministrators { get; set; }
        public DbSet<SystemAdministrator> SystemAdministrators { get; set; }
        #endregion

        #region "Other entities"
        public DbSet<City> Cities { get; set; }
        public DbSet<Dormitory> Dormitories { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<University> Universities { get; set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
