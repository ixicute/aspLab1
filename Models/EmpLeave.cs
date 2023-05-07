using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkolprojektLab1.Models
{
    public class EmpLeave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employees")]
        public int FK_Employee { get; set; }
        public virtual Employee Employees { get; set; }

        [Required]
        [ForeignKey("Leaves")]
        public int FK_Leave { get; set; }
        public virtual Leave Leaves { get; set; }

        [StringLength(maximumLength: 300)]
        public string LeaveDescription { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
