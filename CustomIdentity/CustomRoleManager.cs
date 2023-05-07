using Microsoft.AspNetCore.Identity;
using SkolprojektLab1.Models;

namespace SkolprojektLab1.CustomIdentity
{
    public class CustomRoleManager : RoleManager<Role>
    {
        public CustomRoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
