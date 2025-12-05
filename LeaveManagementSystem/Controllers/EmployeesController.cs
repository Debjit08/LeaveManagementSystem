using Microsoft.AspNetCore.Mvc;
using DataModels;
using DataService;
using ViewModels;

namespace LeaveManagementSystem.Controllers
{
    /// <summary>
    /// Employees Controller - Manage employee information
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILeaveDataService _dataService;

        /// <summary>
        /// Initialize the Employees Controller
        /// </summary>
        public EmployeesController(ILeaveDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        /// <response code="200">Returns list of employees</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<EmployeeViewModel>>>> GetAllEmployees()
        {
            try
            {
                var employees = await _dataService.GetAllEmployeesAsync();
                var employeeVMs = employees.Select(e => new EmployeeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Department = e.Department,
                    JoinDate = e.JoinDate
                }).ToList();

                return Ok(new ApiResponse<List<EmployeeViewModel>>
                {
                    Success = true,
                    Message = "Employees retrieved successfully",
                    Data = employeeVMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving employees: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<EmployeeViewModel>>> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _dataService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found",
                        Data = null
                    });
                }

                var employeeVM = new EmployeeViewModel
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Department = employee.Department,
                    JoinDate = employee.JoinDate
                };

                return Ok(new ApiResponse<EmployeeViewModel>
                {
                    Success = true,
                    Message = "Employee retrieved successfully",
                    Data = employeeVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error retrieving employee: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="employeeVM">Employee data to create</param>
        /// <returns>Created employee</returns>
        /// <response code="201">Employee created successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<EmployeeViewModel>>> CreateEmployee([FromBody] EmployeeViewModel employeeVM)
        {
            try
            {
                var employee = new Employee
                {
                    FirstName = employeeVM.FirstName,
                    LastName = employeeVM.LastName,
                    Email = employeeVM.Email,
                    Department = employeeVM.Department,
                    JoinDate = employeeVM.JoinDate
                };

                var createdEmployee = await _dataService.CreateEmployeeAsync(employee);

                var resultVM = new EmployeeViewModel
                {
                    EmployeeId = createdEmployee.EmployeeId,
                    FirstName = createdEmployee.FirstName,
                    LastName = createdEmployee.LastName,
                    Email = createdEmployee.Email,
                    Department = createdEmployee.Department,
                    JoinDate = createdEmployee.JoinDate
                };

                return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.EmployeeId }, new ApiResponse<EmployeeViewModel>
                {
                    Success = true,
                    Message = "Employee created successfully",
                    Data = resultVM
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error creating employee: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="employeeVM">Updated employee data</param>
        /// <returns>Update result</returns>
        /// <response code="200">Employee updated successfully</response>
        /// <response code="404">Employee not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateEmployee(int id, [FromBody] EmployeeViewModel employeeVM)
        {
            try
            {
                var employee = new Employee
                {
                    EmployeeId = id,
                    FirstName = employeeVM.FirstName,
                    LastName = employeeVM.LastName,
                    Email = employeeVM.Email,
                    Department = employeeVM.Department,
                    JoinDate = employeeVM.JoinDate
                };

                var success = await _dataService.UpdateEmployeeAsync(id, employee);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Employee updated successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error updating employee: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Deletion result</returns>
        /// <response code="200">Employee deleted successfully</response>
        /// <response code="404">Employee not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteEmployee(int id)
        {
            try
            {
                var success = await _dataService.DeleteEmployeeAsync(id);

                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Employee deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error deleting employee: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
