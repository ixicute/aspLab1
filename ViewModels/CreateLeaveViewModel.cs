using SkolprojektLab1.Models;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkolprojektLab1.ViewModels
{
    //server-side validation
    public class CreateLeaveViewModel
    {
        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public List<Leave> leaveList { get; set; }

        public string? LeaveDescription { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public DateOnly CreatedOn { get; set; }
    }
}
