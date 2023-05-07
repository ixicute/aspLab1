namespace SkolprojektLab1.ViewModels
{
    public class SearchEmpLeaveViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public string EmpAddress { get; set; }
        public bool? HasLeave { get; set; }
    }
}
