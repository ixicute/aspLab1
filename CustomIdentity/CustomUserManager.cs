using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkolprojektLab1.Data;
using SkolprojektLab1.Models;

namespace SkolprojektLab1.CustomIdentity
{
    public class CustomUserManager : UserManager<Employee>
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext appDbContext;

        public CustomUserManager(IUserStore<Employee> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<Employee> passwordHasher, IEnumerable<IUserValidator<Employee>> userValidators,
        IEnumerable<IPasswordValidator<Employee>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Employee>> logger,
        DbContextOptions<ApplicationDbContext> options)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
            errors, services, logger)
        {
            _options = options;
        }

        public override async Task<IList<string>> GetRolesAsync(Employee user)
        {
            using (var context = new ApplicationDbContext(_options))
            {
                var roles = await context.Roles.Join(
                    context.Employees.Where(e => e.Id == user.Id),
                    r => r.Id,
                    e => e.FK_Role,
                    (r, e) => r.Name)
                    .ToListAsync();

                return roles;
            }
        }
    }
}
