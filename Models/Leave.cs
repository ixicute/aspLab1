using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkolprojektLab1.Models
{
    public class Leave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(maximumLength: 30)]
        public string LeaveType { get; set; }

        public virtual ICollection<EmpLeave>? EmpLeaves { get; set; }
    }
}
