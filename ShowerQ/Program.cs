//#define CREATE_TESTING_INTERVAL

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShowerQ.Models;
using System.Diagnostics;

namespace ShowerQ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var services = host.Services.CreateScope();

            using var dbContext = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

#if DEBUG
            var userManager = services.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();
            string adminPhoneNumber = "+380 123";

            var u = userManager.Users.FirstOrDefaultAsync(usr => usr.PhoneNumber.Equals(adminPhoneNumber)).Result;

            if(u is default(IdentityUser))
            {
                var user = new IdentityUser() { UserName = "admin", PhoneNumber = adminPhoneNumber };
                var result = userManager.CreateAsync(user, "pass123").GetAwaiter().GetResult();

                if(!result.Succeeded)
                {
                    Debug.WriteLine("System administrator doesn't created.\nErrors:");

                    foreach(var error in result.Errors)
                    {
                        Debug.WriteLine(error.Description);
                    }
                }
                userManager.AddToRoleAsync(user, "SystemAdministrator").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, "DormitoryAdministrator").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, "Tenant").GetAwaiter().GetResult();
                dbContext.SaveChanges();
            }
#endif

#if DEBUG && CREATE_TESTING_INTERVAL
            var interval = new Interval()
            {
                Beginning = DateTime.Now,
                Finishing = DateTime.Now.AddMinutes(10),
                ScheduleId = 8
            };

            dbContext.Intervals.Add(interval);

            dbContext.SaveChanges();
#endif

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
