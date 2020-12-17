using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ShowerQ.Models;
using ShowerQ.Services;

namespace ShowerQ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connection));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config =>
                {
                    var keyStr = Configuration["Security:Key"];
                    var keyBytesArr = Encoding.UTF8.GetBytes(keyStr);
                    var key = new SymmetricSecurityKey(keyBytesArr);

                    config.Events = new JwtBearerEvents()
                    {
                        OnForbidden = context =>
                        {
                            Debug.WriteLine("=======================  TOKEN FORBIDDEN  =======================");
                            return Task.CompletedTask;
                        }
                    };

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = Configuration["Security:JWT:Issuer"],
                        ValidAudience = Configuration["Security:JWT:Audience"],
                        IssuerSigningKey = key,
                    };
                });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSingleton<IPhoneNumberFormatter, PhoneNumberFormatter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateRoles(serviceProvider);
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Tenant", "DormitoryAdministrator", "SystemAdministrator" };

            foreach (var roleName in roleNames)
            {
                var roleExistsTask = roleManager.RoleExistsAsync(roleName);
                roleExistsTask.Wait();

                if (!roleExistsTask.Result)
                {
                    var createRoleTask = roleManager.CreateAsync(new IdentityRole(roleName));
                    createRoleTask.Wait();
                }
            }
        }
    }
}
