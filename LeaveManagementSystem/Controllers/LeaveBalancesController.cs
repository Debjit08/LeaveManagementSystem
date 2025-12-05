using Microsoft.AspNetCore.Mvc;
using DataModels;
using DataService;
using ViewModels;

namespace LeaveManagementSystem.Controllers
{
    /// <summary>
    /// Leave Balances Controller - Manage employee leave balances and allocations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveBalancesController : ControllerBase
    {
        private readonly ILeaveDataService _dataService;

        /// <summary>
        /// Initialize the Leave Balances Controller
        /// </summary>
        public LeaveBalancesController(ILeaveDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Get all leave balances
        /// </summary>
        /// <returns>List of all leave balances</returns>
        /// <response code="200">Returns list of leave balances</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LeaveBalanceViewModel>>>> GetAllLeaveBalances()
        {
            try
            {
                var balances = await _dataService.GetAllLeaveBalancesAsync();
                var employees = await _dataService.GetAllEmployeesAsync();

                var balanceVMs = balances.Select(b =>
                {
                    var employee = employees.FirstOrDefault(e => e.EmployeeId == b.EmployeeId);
                    return new LeaveBalanceViewModel
                    {
                        LeaveBalanceId = b.LeaveBalanceId,
                        EmployeeId = b.EmployeeId,
                        EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
                        LeaveType = b.LeaveType,
                        TotalDays = b.TotalDays,
                        UsedDays = b.UsedDays,
                        RemainingDays = b.RemainingDays,
                        Year = b.Year
                    };
                }).ToList();

                return Ok(new ApiResponse<List<LeaveBalanceViewModel>>
                {
                    Success = true,
                    Message = "Leave balances retrieved successfully",
                    Data = balanceVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leave balances: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get leave balance by ID
        /// </summary>
        /// <param name="id">Leave Balance ID</param>
        /// <returns>Leave balance details</returns>
        /// <response code="200">Leave balance found</response>
        /// <response code="404">Leave balance not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LeaveBalanceViewModel>>> GetLeaveBalanceById(int id)
        {
            try
            {
                var balance = await _dataService.GetLeaveBalanceByIdAsync(id);
                if (balance == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave balance with ID {id} not found",
                        Data = null
                    });
                }

                var employee = await _dataService.GetEmployeeByIdAsync(balance.EmployeeId);

                var balanceVM = new LeaveBalanceViewModel
                {
                    LeaveBalanceId = balance.LeaveBalanceId,
                    EmployeeId = balance.EmployeeId,
                    EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
                    LeaveType = balance.LeaveType,
                    TotalDays = balance.TotalDays,
                    UsedDays = balance.UsedDays,
                    RemainingDays = balance.RemainingDays,
                    Year = balance.Year
                };

                return Ok(new ApiResponse<LeaveBalanceViewModel>
                {
                    Success = true,
                    Message = "Leave balance retrieved successfully",
                    Data = balanceVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leave balance: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get all leave balances for a specific employee
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>List of employee's leave balances</returns>
        /// <response code="200">Leave balances found</response>
        /// <response code="404">Employee not found</response>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<LeaveBalanceViewModel>>>> GetLeaveBalancesByEmployeeId(int employeeId)
        {
            try
            {
                var balances = await _dataService.GetLeaveBalancesByEmployeeIdAsync(employeeId);
                var employee = await _dataService.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {employeeId} not found",
                        Data = null
                    });
                }

                var balanceVMs = balances.Select(b => new LeaveBalanceViewModel
                {
                    LeaveBalanceId = b.LeaveBalanceId,
                    EmployeeId = b.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    LeaveType = b.LeaveType,
                    TotalDays = b.TotalDays,
                    UsedDays = b.UsedDays,
                    RemainingDays = b.RemainingDays,
                    Year = b.Year
                }).ToList();

                return Ok(new ApiResponse<List<LeaveBalanceViewModel>>
                {
                    Success = true,
                    Message = "Leave balances retrieved successfully",
                    Data = balanceVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leave balances: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get leave balances for a specific employee and year
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="year">Year</param>
        /// <returns>Leave balances for the specified year</returns>
        /// <response code="200">Leave balances found</response>
        /// <response code="404">Employee not found</response>
        [HttpGet("employee/{employeeId}/year/{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<LeaveBalanceViewModel>>>> GetLeaveBalancesByEmployeeAndYear(int employeeId, int year)
        {
            try
            {
                var balances = await _dataService.GetLeaveBalancesByEmployeeIdAsync(employeeId);
                var employee = await _dataService.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {employeeId} not found",
                        Data = null
                    });
                }

                var balanceVMs = balances.Where(b => b.Year == year).Select(b => new LeaveBalanceViewModel
                {
                    LeaveBalanceId = b.LeaveBalanceId,
                    EmployeeId = b.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    LeaveType = b.LeaveType,
                    TotalDays = b.TotalDays,
                    UsedDays = b.UsedDays,
                    RemainingDays = b.RemainingDays,
                    Year = b.Year
                }).ToList();

                return Ok(new ApiResponse<List<LeaveBalanceViewModel>>
                {
                    Success = true,
                    Message = "Leave balances retrieved successfully",
                    Data = balanceVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leave balances: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Create a new leave balance
        /// </summary>
        /// <param name="balanceVM">Leave balance details</param>
        /// <returns>Created leave balance</returns>
        /// <response code="201">Leave balance created successfully</response>
        /// <response code="404">Employee not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LeaveBalanceViewModel>>> CreateLeaveBalance([FromBody] LeaveBalanceViewModel balanceVM)
        {
            try
            {
                var employee = await _dataService.GetEmployeeByIdAsync(balanceVM.EmployeeId);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {balanceVM.EmployeeId} not found",
                        Data = null
                    });
                }

                var balance = new LeaveBalance
                {
                    EmployeeId = balanceVM.EmployeeId,
                    LeaveType = balanceVM.LeaveType,
                    TotalDays = balanceVM.TotalDays,
                    UsedDays = balanceVM.UsedDays,
                    RemainingDays = balanceVM.RemainingDays,
                    Year = balanceVM.Year
                };

                var createdBalance = await _dataService.CreateLeaveBalanceAsync(balance);

                var resultVM = new LeaveBalanceViewModel
                {
                    LeaveBalanceId = createdBalance.LeaveBalanceId,
                    EmployeeId = createdBalance.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    LeaveType = createdBalance.LeaveType,
                    TotalDays = createdBalance.TotalDays,
                    UsedDays = createdBalance.UsedDays,
                    RemainingDays = createdBalance.RemainingDays,
                    Year = createdBalance.Year
                };

                return CreatedAtAction(nameof(GetLeaveBalanceById), new { id = createdBalance.LeaveBalanceId }, new ApiResponse<LeaveBalanceViewModel>
                {
                    Success = true,
                    Message = "Leave balance created successfully",
                    Data = resultVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error creating leave balance: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update an existing leave balance
        /// </summary>
        /// <param name="id">Leave Balance ID</param>
        /// <param name="balanceVM">Updated leave balance data</param>
        /// <returns>Update result</returns>
        /// <response code="200">Leave balance updated successfully</response>
        /// <response code="404">Leave balance not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateLeaveBalance(int id, [FromBody] LeaveBalanceViewModel balanceVM)
        {
            try
            {
                var balance = new LeaveBalance
                {
                    LeaveBalanceId = id,
                    EmployeeId = balanceVM.EmployeeId,
                    LeaveType = balanceVM.LeaveType,
                    TotalDays = balanceVM.TotalDays,
                    UsedDays = balanceVM.UsedDays,
                    RemainingDays = balanceVM.RemainingDays,
                    Year = balanceVM.Year
                };

                var success = await _dataService.UpdateLeaveBalanceAsync(id, balance);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave balance with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Leave balance updated successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error updating leave balance: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Delete a leave balance
        /// </summary>
        /// <param name="id">Leave Balance ID</param>
        /// <returns>Deletion result</returns>
        /// <response code="200">Leave balance deleted successfully</response>
        /// <response code="404">Leave balance not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLeaveBalance(int id)
        {
            try
            {
                var success = await _dataService.DeleteLeaveBalanceAsync(id);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave balance with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Leave balance deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error deleting leave balance: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
