# Leave Management System - Postman Collection

This folder contains a complete Postman collection for testing the Leave Management System API.

## Files Included

1. **Leave_Management_API.postman_collection.json** - Complete API collection with all endpoints
2. **Leave_Management_Environment.postman_environment.json** - Environment variables for easy configuration
3. **README.md** - This file with setup and usage instructions

## Setup Instructions

### 1. Import Collection and Environment into Postman

1. Open Postman
2. Click **Import** button
3. Select **Leave_Management_API.postman_collection.json**
4. Click **Import**
5. Repeat steps 2-4 for **Leave_Management_Environment.postman_environment.json**

### 2. Set Active Environment

1. In Postman, locate the environment dropdown (usually top-right)
2. Select **Leave Management System Environment**

### 3. Configure Base URL (if needed)

If your API is running on a different port or host:
1. Click on the environment dropdown
2. Select **Edit**
3. Modify the `base_url` variable to match your API endpoint
4. Save

## API Endpoints Available

### Employees Management
- **GET** `/api/employees` - Get all employees
- **GET** `/api/employees/{id}` - Get employee by ID
- **POST** `/api/employees` - Create new employee
- **PUT** `/api/employees/{id}` - Update employee
- **DELETE** `/api/employees/{id}` - Delete employee

### Leaves Management
- **GET** `/api/leaves` - Get all leave requests
- **GET** `/api/leaves/{id}` - Get leave request by ID
- **GET** `/api/leaves/employee/{employeeId}` - Get leaves for specific employee
- **GET** `/api/leaves/status/{status}` - Get leaves by status (Pending, Approved, Rejected)
- **POST** `/api/leaves` - Create new leave request
- **PUT** `/api/leaves/{id}` - Update leave request (including approval)
- **DELETE** `/api/leaves/{id}` - Delete leave request

### Leave Balances Management
- **GET** `/api/leavebalances` - Get all leave balances
- **GET** `/api/leavebalances/{id}` - Get leave balance by ID
- **GET** `/api/leavebalances/employee/{employeeId}` - Get balances for employee
- **GET** `/api/leavebalances/employee/{employeeId}/year/{year}` - Get balances by year
- **POST** `/api/leavebalances` - Create new leave balance
- **PUT** `/api/leavebalances/{id}` - Update leave balance
- **DELETE** `/api/leavebalances/{id}` - Delete leave balance

## Testing Workflows

### 1. Employee Management Workflow
1. Execute **Get All Employees** - View all employees
2. Execute **Get Employee By ID** - View specific employee (ID: 1)
3. Execute **Create Employee** - Add new employee
4. Execute **Update Employee** - Modify employee details
5. Execute **Delete Employee** - Remove employee (use new ID)

### 2. Leave Request Workflow
1. Execute **Get All Leaves** - View all leave requests
2. Execute **Get Leaves by Employee** - Filter by employee (ID: 1)
3. Execute **Get Leaves by Status** - Filter by status (e.g., Pending)
4. Execute **Create Leave Request** - Submit new leave request
5. Execute **Update Leave Request** - Approve/reject or modify request
6. Execute **Get Leave By ID** - View updated request

### 3. Leave Balance Workflow
1. Execute **Get All Leave Balances** - View all balances
2. Execute **Get Leave Balances by Employee** - View employee's balances
3. Execute **Get Leave Balances by Employee and Year** - Check specific year
4. Execute **Create Leave Balance** - Allocate new balance
5. Execute **Update Leave Balance** - Adjust used/remaining days
6. Execute **Delete Leave Balance** - Remove allocation

## Response Format

All API responses follow this standard format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    // Response data here
  }
}
```

## Sample Data

The system comes pre-populated with:

### Employees (5 total)
- John Doe (ID: 1) - Engineering
- Jane Smith (ID: 2) - HR
- Michael Johnson (ID: 3) - Finance
- Emily Williams (ID: 4) - Engineering
- David Brown (ID: 5) - Marketing

### Leaves (5 sample requests)
- Various statuses: Pending, Approved, Rejected
- Different leave types: Annual, Sick, Casual

### Leave Balances (15 total)
- 3 leave types per employee: Annual, Sick, Casual
- Year: 2025

## Environment Variables

The collection uses these variables:

| Variable | Default | Usage |
|----------|---------|-------|
| `base_url` | http://localhost:5233 | Base URL for all requests |
| `employee_id` | 1 | Default employee ID for testing |
| `leave_id` | 1 | Default leave ID for testing |
| `leave_balance_id` | 1 | Default balance ID for testing |

## Tips for Testing

1. **Use Variables**: Click on `{{variable_name}}` in URLs/bodies to use environment variables
2. **Check Responses**: Click on response tabs to see raw, pretty-printed, or formatted views
3. **Test Status Codes**: Verify responses include correct HTTP status codes
4. **Save Responses**: Use the response preview to save sample responses
5. **Create Test Scripts**: Add JavaScript in Tests tab for automated validation

## Common Test Scenarios

### Create and Approve Leave
1. Create a new leave request (POST /api/leaves)
2. Copy the returned LeaveId
3. Update the leave request with status "Approved" (PUT /api/leaves/{id})
4. Verify the status changed

### Update Leave Balances After Approval
1. Get employee's leave balance (GET /api/leavebalances/employee/{id})
2. Calculate new used days (add days from approved leave)
3. Update the balance with new used/remaining days (PUT /api/leavebalances/{id})

### Generate Reports
1. Get all leaves with status "Approved"
2. Get all employees
3. Cross-reference to create employee-wise leave report

## Error Handling

The API returns appropriate HTTP status codes:

- **200 OK** - Successful GET/PUT operation
- **201 Created** - Successful POST operation
- **404 Not Found** - Resource doesn't exist
- **400 Bad Request** - Invalid request data
- **500 Internal Server Error** - Server error

Check the response body for error messages in `message` field.

## API Performance Notes

- The API uses a JSON file-based database
- All operations are asynchronous
- Response times are typically < 100ms
- Maximum concurrent requests: Limited by system resources

## Support

For issues or questions:
1. Verify API is running on the correct port
2. Check environment variables are set correctly
3. Review API documentation in code
4. Check OpenAPI spec at `/openapi/v1.json`

---

**Last Updated**: December 5, 2025
**API Version**: 1.0
**Target Framework**: .NET 10
