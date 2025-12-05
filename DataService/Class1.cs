using System.Text.Json;
using DataModels;

namespace DataService
{
    public interface ILeaveDataService
    {
        // Employee operations
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);

        // Leave operations
        Task<List<Leave>> GetAllLeavesAsync();
        Task<Leave> GetLeaveByIdAsync(int leaveId);
        Task<List<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task<List<Leave>> GetLeavesByStatusAsync(string status);
        Task<Leave> CreateLeaveAsync(Leave leave);
        Task<bool> UpdateLeaveAsync(int leaveId, Leave leave);
        Task<bool> DeleteLeaveAsync(int leaveId);

        // Leave Balance operations
        Task<List<LeaveBalance>> GetAllLeaveBalancesAsync();
        Task<LeaveBalance> GetLeaveBalanceByIdAsync(int leaveBalanceId);
        Task<List<LeaveBalance>> GetLeaveBalancesByEmployeeIdAsync(int employeeId);
        Task<LeaveBalance> GetLeaveBalanceAsync(int employeeId, string leaveType, int year);
        Task<LeaveBalance> CreateLeaveBalanceAsync(LeaveBalance leaveBalance);
        Task<bool> UpdateLeaveBalanceAsync(int leaveBalanceId, LeaveBalance leaveBalance);
        Task<bool> DeleteLeaveBalanceAsync(int leaveBalanceId);
    }

    public class LeaveDataService : ILeaveDataService
    {
        private readonly string _databasePath;
        private readonly object _lockObject = new object();

        public LeaveDataService(string databasePath)
        {
            _databasePath = databasePath;
        }

        private async Task<Database> LoadDatabaseAsync()
        {
            lock (_lockObject)
            {
                if (!File.Exists(_databasePath))
                {
                    throw new FileNotFoundException($"Database file not found: {_databasePath}");
                }

                string jsonContent = File.ReadAllText(_databasePath);
                return JsonSerializer.Deserialize<Database>(jsonContent) ?? new Database();
            }
        }

        private async Task SaveDatabaseAsync(Database database)
        {
            lock (_lockObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(database, options);
                File.WriteAllText(_databasePath, jsonContent);
            }
        }

        // Employee operations
        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var db = await LoadDatabaseAsync();
            return db.Employees ?? new List<Employee>();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var employees = await GetAllEmployeesAsync();
            return employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            var db = await LoadDatabaseAsync();
            var maxId = db.Employees?.Max(e => e.EmployeeId) ?? 0;
            employee.EmployeeId = maxId + 1;
            db.Employees ??= new List<Employee>();
            db.Employees.Add(employee);
            await SaveDatabaseAsync(db);
            return employee;
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            var db = await LoadDatabaseAsync();
            var existingEmployee = db.Employees?.FirstOrDefault(e => e.EmployeeId == employeeId);
            if (existingEmployee == null) return false;

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.JoinDate = employee.JoinDate;

            await SaveDatabaseAsync(db);
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var db = await LoadDatabaseAsync();
            var employee = db.Employees?.FirstOrDefault(e => e.EmployeeId == employeeId);
            if (employee == null) return false;

            db.Employees.Remove(employee);
            await SaveDatabaseAsync(db);
            return true;
        }

        // Leave operations
        public async Task<List<Leave>> GetAllLeavesAsync()
        {
            var db = await LoadDatabaseAsync();
            return db.Leaves ?? new List<Leave>();
        }

        public async Task<Leave> GetLeaveByIdAsync(int leaveId)
        {
            var leaves = await GetAllLeavesAsync();
            return leaves.FirstOrDefault(l => l.LeaveId == leaveId);
        }

        public async Task<List<Leave>> GetLeavesByEmployeeIdAsync(int employeeId)
        {
            var leaves = await GetAllLeavesAsync();
            return leaves.Where(l => l.EmployeeId == employeeId).ToList();
        }

        public async Task<List<Leave>> GetLeavesByStatusAsync(string status)
        {
            var leaves = await GetAllLeavesAsync();
            return leaves.Where(l => l.Status == status).ToList();
        }

        public async Task<Leave> CreateLeaveAsync(Leave leave)
        {
            var db = await LoadDatabaseAsync();
            var maxId = db.Leaves?.Max(l => l.LeaveId) ?? 0;
            leave.LeaveId = maxId + 1;
            leave.AppliedDate = DateTime.UtcNow;
            leave.Status = "Pending";

            db.Leaves ??= new List<Leave>();
            db.Leaves.Add(leave);
            await SaveDatabaseAsync(db);
            return leave;
        }

        public async Task<bool> UpdateLeaveAsync(int leaveId, Leave leave)
        {
            var db = await LoadDatabaseAsync();
            var existingLeave = db.Leaves?.FirstOrDefault(l => l.LeaveId == leaveId);
            if (existingLeave == null) return false;

            existingLeave.StartDate = leave.StartDate;
            existingLeave.EndDate = leave.EndDate;
            existingLeave.LeaveType = leave.LeaveType;
            existingLeave.Status = leave.Status;
            existingLeave.Reason = leave.Reason;

            await SaveDatabaseAsync(db);
            return true;
        }

        public async Task<bool> DeleteLeaveAsync(int leaveId)
        {
            var db = await LoadDatabaseAsync();
            var leave = db.Leaves?.FirstOrDefault(l => l.LeaveId == leaveId);
            if (leave == null) return false;

            db.Leaves.Remove(leave);
            await SaveDatabaseAsync(db);
            return true;
        }

        // Leave Balance operations
        public async Task<List<LeaveBalance>> GetAllLeaveBalancesAsync()
        {
            var db = await LoadDatabaseAsync();
            return db.LeaveBalances ?? new List<LeaveBalance>();
        }

        public async Task<LeaveBalance> GetLeaveBalanceByIdAsync(int leaveBalanceId)
        {
            var balances = await GetAllLeaveBalancesAsync();
            return balances.FirstOrDefault(b => b.LeaveBalanceId == leaveBalanceId);
        }

        public async Task<List<LeaveBalance>> GetLeaveBalancesByEmployeeIdAsync(int employeeId)
        {
            var balances = await GetAllLeaveBalancesAsync();
            return balances.Where(b => b.EmployeeId == employeeId).ToList();
        }

        public async Task<LeaveBalance> GetLeaveBalanceAsync(int employeeId, string leaveType, int year)
        {
            var balances = await GetAllLeaveBalancesAsync();
            return balances.FirstOrDefault(b => b.EmployeeId == employeeId && b.LeaveType == leaveType && b.Year == year);
        }

        public async Task<LeaveBalance> CreateLeaveBalanceAsync(LeaveBalance leaveBalance)
        {
            var db = await LoadDatabaseAsync();
            var maxId = db.LeaveBalances?.Max(b => b.LeaveBalanceId) ?? 0;
            leaveBalance.LeaveBalanceId = maxId + 1;

            db.LeaveBalances ??= new List<LeaveBalance>();
            db.LeaveBalances.Add(leaveBalance);
            await SaveDatabaseAsync(db);
            return leaveBalance;
        }

        public async Task<bool> UpdateLeaveBalanceAsync(int leaveBalanceId, LeaveBalance leaveBalance)
        {
            var db = await LoadDatabaseAsync();
            var existingBalance = db.LeaveBalances?.FirstOrDefault(b => b.LeaveBalanceId == leaveBalanceId);
            if (existingBalance == null) return false;

            existingBalance.TotalDays = leaveBalance.TotalDays;
            existingBalance.UsedDays = leaveBalance.UsedDays;
            existingBalance.RemainingDays = leaveBalance.RemainingDays;

            await SaveDatabaseAsync(db);
            return true;
        }

        public async Task<bool> DeleteLeaveBalanceAsync(int leaveBalanceId)
        {
            var db = await LoadDatabaseAsync();
            var balance = db.LeaveBalances?.FirstOrDefault(b => b.LeaveBalanceId == leaveBalanceId);
            if (balance == null) return false;

            db.LeaveBalances.Remove(balance);
            await SaveDatabaseAsync(db);
            return true;
        }
    }

    // Helper class for JSON deserialization
    public class Database
    {
        public List<Employee>? Employees { get; set; }
        public List<Leave>? Leaves { get; set; }
        public List<LeaveBalance>? LeaveBalances { get; set; }
    }
}
