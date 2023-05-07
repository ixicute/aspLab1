using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using SkolprojektLab1.Models;
using Microsoft.AspNetCore.Identity;

namespace SkolprojektLab1.Models
{
    public class Employee : IdentityUser<int>
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string LastName { get; set; }

        [ForeignKey("Addresses")]
        public int FK_Adress { get; set; }
        public virtual Address Addresses { get; set; }


        [ForeignKey("Roles")]
        public int FK_Role { get; set; }
        public virtual Role Roles { get; set; }

        public virtual ICollection<EmpLeave>? EmpLeaves { get; set; }
    }
}
