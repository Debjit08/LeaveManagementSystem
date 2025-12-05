using Microsoft.AspNetCore.Mvc;
using DataModels;
using DataService;
using ViewModels;

namespace LeaveManagementSystem.Controllers
{
    /// <summary>
    /// Leaves Controller - Manage leave requests and approvals
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LeavesController : ControllerBase
    {
        private readonly ILeaveDataService _dataService;

        /// <summary>
        /// Initialize the Leaves Controller
        /// </summary>
        public LeavesController(ILeaveDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Get all leave requests
        /// </summary>
        /// <returns>List of all leave requests</returns>
        /// <response code="200">Returns list of leaves</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<LeaveViewModel>>>> GetAllLeaves()
        {
            try
            {
                var leaves = await _dataService.GetAllLeavesAsync();
                var employees = await _dataService.GetAllEmployeesAsync();

                var leaveVMs = new List<LeaveViewModel>();
                foreach (var leave in leaves)
                {
                    var employee = employees.FirstOrDefault(e => e.EmployeeId == leave.EmployeeId);
                    var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;

                    leaveVMs.Add(new LeaveViewModel
                    {
                        LeaveId = leave.LeaveId,
                        EmployeeId = leave.EmployeeId,
                        EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
                        StartDate = leave.StartDate,
                        EndDate = leave.EndDate,
                        TotalDays = totalDays,
                        LeaveType = leave.LeaveType,
                        Status = leave.Status,
                        Reason = leave.Reason,
                        AppliedDate = leave.AppliedDate
                    });
                }

                return Ok(new ApiResponse<List<LeaveViewModel>>
                {
                    Success = true,
                    Message = "Leaves retrieved successfully",
                    Data = leaveVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leaves: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get leave request by ID
        /// </summary>
        /// <param name="id">Leave ID</param>
        /// <returns>Leave details</returns>
        /// <response code="200">Leave found</response>
        /// <response code="404">Leave not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LeaveViewModel>>> GetLeaveById(int id)
        {
            try
            {
                var leave = await _dataService.GetLeaveByIdAsync(id);
                if (leave == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave with ID {id} not found",
                        Data = null
                    });
                }

                var employee = await _dataService.GetEmployeeByIdAsync(leave.EmployeeId);
                var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;

                var leaveVM = new LeaveViewModel
                {
                    LeaveId = leave.LeaveId,
                    EmployeeId = leave.EmployeeId,
                    EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    TotalDays = totalDays,
                    LeaveType = leave.LeaveType,
                    Status = leave.Status,
                    Reason = leave.Reason,
                    AppliedDate = leave.AppliedDate
                };

                return Ok(new ApiResponse<LeaveViewModel>
                {
                    Success = true,
                    Message = "Leave retrieved successfully",
                    Data = leaveVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leave: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get all leave requests for a specific employee
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>List of employee's leave requests</returns>
        /// <response code="200">Leaves found</response>
        /// <response code="404">Employee not found</response>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<LeaveViewModel>>>> GetLeavesByEmployeeId(int employeeId)
        {
            try
            {
                var leaves = await _dataService.GetLeavesByEmployeeIdAsync(employeeId);
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

                var leaveVMs = leaves.Select(leave =>
                {
                    var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
                    return new LeaveViewModel
                    {
                        LeaveId = leave.LeaveId,
                        EmployeeId = leave.EmployeeId,
                        EmployeeName = $"{employee.FirstName} {employee.LastName}",
                        StartDate = leave.StartDate,
                        EndDate = leave.EndDate,
                        TotalDays = totalDays,
                        LeaveType = leave.LeaveType,
                        Status = leave.Status,
                        Reason = leave.Reason,
                        AppliedDate = leave.AppliedDate
                    };
                }).ToList();

                return Ok(new ApiResponse<List<LeaveViewModel>>
                {
                    Success = true,
                    Message = "Leaves retrieved successfully",
                    Data = leaveVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leaves: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get all leave requests with a specific status (Pending, Approved, Rejected)
        /// </summary>
        /// <param name="status">Leave status (Pending, Approved, Rejected)</param>
        /// <returns>List of leaves with specified status</returns>
        /// <response code="200">Leaves found</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LeaveViewModel>>>> GetLeavesByStatus(string status)
        {
            try
            {
                var leaves = await _dataService.GetLeavesByStatusAsync(status);
                var employees = await _dataService.GetAllEmployeesAsync();

                var leaveVMs = new List<LeaveViewModel>();
                foreach (var leave in leaves)
                {
                    var employee = employees.FirstOrDefault(e => e.EmployeeId == leave.EmployeeId);
                    var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;

                    leaveVMs.Add(new LeaveViewModel
                    {
                        LeaveId = leave.LeaveId,
                        EmployeeId = leave.EmployeeId,
                        EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
                        StartDate = leave.StartDate,
                        EndDate = leave.EndDate,
                        TotalDays = totalDays,
                        LeaveType = leave.LeaveType,
                        Status = leave.Status,
                        Reason = leave.Reason,
                        AppliedDate = leave.AppliedDate
                    });
                }

                return Ok(new ApiResponse<List<LeaveViewModel>>
                {
                    Success = true,
                    Message = "Leaves retrieved successfully",
                    Data = leaveVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving leaves: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Create a new leave request
        /// </summary>
        /// <param name="createLeaveVM">Leave request details</param>
        /// <returns>Created leave request</returns>
        /// <response code="201">Leave created successfully</response>
        /// <response code="404">Employee not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LeaveViewModel>>> CreateLeave([FromBody] CreateLeaveRequestViewModel createLeaveVM)
        {
            try
            {
                var employee = await _dataService.GetEmployeeByIdAsync(createLeaveVM.EmployeeId);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {createLeaveVM.EmployeeId} not found",
                        Data = null
                    });
                }

                var leave = new Leave
                {
                    EmployeeId = createLeaveVM.EmployeeId,
                    StartDate = createLeaveVM.StartDate,
                    EndDate = createLeaveVM.EndDate,
                    LeaveType = createLeaveVM.LeaveType,
                    Reason = createLeaveVM.Reason
                };

                var createdLeave = await _dataService.CreateLeaveAsync(leave);
                var totalDays = (int)(createdLeave.EndDate - createdLeave.StartDate).TotalDays + 1;

                var leaveVM = new LeaveViewModel
                {
                    LeaveId = createdLeave.LeaveId,
                    EmployeeId = createdLeave.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    StartDate = createdLeave.StartDate,
                    EndDate = createdLeave.EndDate,
                    TotalDays = totalDays,
                    LeaveType = createdLeave.LeaveType,
                    Status = createdLeave.Status,
                    Reason = createdLeave.Reason,
                    AppliedDate = createdLeave.AppliedDate
                };

                return CreatedAtAction(nameof(GetLeaveById), new { id = createdLeave.LeaveId }, new ApiResponse<LeaveViewModel>
                {
                    Success = true,
                    Message = "Leave created successfully",
                    Data = leaveVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error creating leave: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update an existing leave request
        /// </summary>
        /// <param name="id">Leave ID</param>
        /// <param name="leaveVM">Updated leave data</param>
        /// <returns>Update result</returns>
        /// <response code="200">Leave updated successfully</response>
        /// <response code="404">Leave not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateLeave(int id, [FromBody] LeaveViewModel leaveVM)
        {
            try
            {
                var leave = new Leave
                {
                    LeaveId = id,
                    EmployeeId = leaveVM.EmployeeId,
                    StartDate = leaveVM.StartDate,
                    EndDate = leaveVM.EndDate,
                    LeaveType = leaveVM.LeaveType,
                    Status = leaveVM.Status,
                    Reason = leaveVM.Reason
                };

                var success = await _dataService.UpdateLeaveAsync(id, leave);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Leave updated successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error updating leave: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Delete a leave request
        /// </summary>
        /// <param name="id">Leave ID</param>
        /// <returns>Deletion result</returns>
        /// <response code="200">Leave deleted successfully</response>
        /// <response code="404">Leave not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLeave(int id)
        {
            try
            {
                var success = await _dataService.DeleteLeaveAsync(id);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Leave with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Leave deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error deleting leave: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
