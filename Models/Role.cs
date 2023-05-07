using Microsoft.AspNetCore.Identity;

namespace SkolprojektLab1.Models
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
