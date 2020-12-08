using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShowerQ.Models.Entities;
using ShowerQ.Models.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Dormitory> Dormitories { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
    }
}
