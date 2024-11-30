using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce_app.Data;
using System;
using Ecommerce_app.Areas.Identity.Data;
using Ecommerce_app.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
namespace Ecommerce_app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<EcommerceAppContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            builder.Services.AddDbContext<UserContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

            builder.Services.AddSingleton<IConnectionMultiplexer>(option => 
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("Connection string 'RedisConnection' not found")));

            builder.Services.AddIdentity<AppUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<UserContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
            });

            /* Setting Authentication scheme for different role.
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultScheme = "UserAuth";
            })
            .AddCookie("UserAuth", options => 
            {
                options.LoginPath = "/";
            })
            .AddCookie("AdminAuth", options => 
            {
                options.LoginPath = "/admin/";
            });*/

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            /*
            using (var scope = app.Services.CreateScope())
            {
            var services = scope.ServiceProvider;
            try
            {
                // add 10 seconds delay to ensure the db server is up to accept connections
                // this won't be needed in real world application
                System.Threading.Thread.Sleep(10000);
                var context = services.GetRequiredService<UserContext>();
                var created = context.Database.EnsureCreated();

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
            }*/

            // Add identity roles to database.
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Member", "Demo" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
/*
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var user = new AppUser { UserName = "admin" };

                if(userManager.FindByNameAsync(user.UserName) == null)
                {
                    var result = await userManager.CreateAsync(user, "admin");
                    await userManager.AddToRoleAsync(user, "Admin");
                }*/
            }

            app.MapControllerRoute(
                name: "MyArea",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
