using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkolprojektLab1.CustomIdentity;
using SkolprojektLab1.Data;
using SkolprojektLab1.Models;

namespace SkolprojektLab1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            //Injecting custom IDentity
            builder.Services.AddIdentity<Employee, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<CustomUserManager>()
                .AddRoleManager<CustomRoleManager>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            Seed.SeedData(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            

            app.Run();
        }
    }
}