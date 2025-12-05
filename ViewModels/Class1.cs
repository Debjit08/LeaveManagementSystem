namespace ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public DateTime JoinDate { get; set; }
    }

    public class LeaveViewModel
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? LeaveType { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public DateTime AppliedDate { get; set; }
    }

    public class LeaveBalanceViewModel
    {
        public int LeaveBalanceId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? LeaveType { get; set; }
        public int TotalDays { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
        public int Year { get; set; }
    }

    public class CreateLeaveRequestViewModel
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? LeaveType { get; set; }
        public string? Reason { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
