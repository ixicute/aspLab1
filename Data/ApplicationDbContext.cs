using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkolprojektLab1.Models;

namespace SkolprojektLab1.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<EmpLeave> EmpLeaves { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityUserRole<int>>();
            modelBuilder.Ignore<IdentityUserToken<int>>();

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.Addresses)
            .WithMany(a => a.Employees)
            .HasForeignKey(e => e.FK_Adress)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Roles)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.FK_Role)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmpLeave>()
                .HasOne(el => el.Employees)
                .WithMany(e => e.EmpLeaves)
                .HasForeignKey(el => el.FK_Employee)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmpLeave>()
                .HasOne(el => el.Leaves)
                .WithMany(l => l.EmpLeaves)
                .HasForeignKey(el => el.FK_Leave)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
