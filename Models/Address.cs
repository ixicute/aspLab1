using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkolprojektLab1.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [StringLength(maximumLength: 100)]
        public string Street { get; set; }


        [StringLength(maximumLength: 30)]
        public string City { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
