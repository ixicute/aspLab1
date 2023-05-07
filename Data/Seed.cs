using Microsoft.AspNetCore.Identity;
using SkolprojektLab1.Models;
using SkolprojektLab1.CustomIdentity;

namespace SkolprojektLab1.Data
{
    public class Seed
    {
        public static async void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var roleManager = serviceScope.ServiceProvider.GetService<CustomRoleManager>();

                var userManager = serviceScope.ServiceProvider.GetService<CustomUserManager>();

                context.Database.EnsureCreated();

                if (!context.Leaves.Any())
                {
                    context.Leaves.AddRange(new List<Leave>()
                    {
                        new Leave()
                        {
                            LeaveType = "Sick"
                        },
                        new Leave()
                        {
                            LeaveType = "Personal"
                        },
                        new Leave()
                        {
                            LeaveType = "Parenting"
                        },
                        new Leave()
                        {
                            LeaveType = "Vacation"
                        }
                    });
                    context.SaveChanges();
                }

                if (!context.Addresses.Any())
                {
                    context.Addresses.AddRange(new List<Address>
                    {
                        new Address()
                        {
                            Street = "Bästagatan 21C",
                            City = "Sundsvall"
                        },
                        new Address()
                        {
                            Street = "Granloholmvägen 10E",
                            City = "Timrå"
                        },
                        new Address()
                        {
                            Street = "Skönsbersgatan 15F",
                            City = "Ånge"
                        }
                    });
                    context.SaveChanges();
                }

                if (!context.Roles.Any())
                {
                    var admin = new Role
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    };
                    await roleManager.CreateAsync(admin);

                    var user = new Role
                    {
                        Name = "User",
                        NormalizedName = "USER"
                    };
                    await roleManager.CreateAsync(user);
                }

                if (!context.Employees.Any())
                {
                    var newAdmin = new Employee()
                    {
                        FirstName = "Admin",
                        LastName = "Aldor",
                        FK_Adress = 1,
                        UserName = "admin",
                        FK_Role = 1,
                        Email = "admin@MA.se",
                        EmailConfirmed = true
                    };

                    //The password could (read, should) techniqually be inserted into the appsettings.json and retrived for security-reasons.
                    //but this is just a training project :D
                    var check = await userManager.CreateAsync(newAdmin, "adminMaX33!");

                    var newUser = new Employee()
                    {
                        FirstName = "User",
                        LastName = "Aldor",
                        FK_Adress = 2,
                        UserName = "user",
                        FK_Role = 2,
                        Email = "user@MA.se",
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(newUser, "userMaX33!");
                }
                
                if (!context.EmpLeaves.Any())
                {
                    context.EmpLeaves.AddRange(new List<EmpLeave>()
                    {
                        new EmpLeave
                        {
                            FK_Employee = 1,
                            FK_Leave = 4,
                            LeaveDescription = "Worked too hard, need a break!",
                            Start = DateTime.Parse("2023-05-05"),
                            End = DateTime.Parse("2023-05-06"),
                            CreatedOn = DateTime.Parse("2023-05-01")
                        },

                        new EmpLeave
                        {
                            FK_Employee = 2,
                            FK_Leave = 2,
                            LeaveDescription = "",
                            Start = DateTime.Parse("2023-05-02"),
                            End = DateTime.Parse("2023-05-05"),
                            CreatedOn = DateTime.Parse("2023-05-01")
                        }
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
