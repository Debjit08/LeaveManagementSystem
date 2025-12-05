# Leave Management System

A comprehensive .NET 9 web API for managing employee leave requests, balances, and employee information.

## Project Structure

The solution follows a clean architecture pattern with the following projects:

### 1. **DataModels** (Class Library)
Contains the core database models:
- `Employee`: Represents employee information
- `Leave`: Represents leave requests
- `LeaveBalance`: Tracks leave days available per employee per leave type per year

### 2. **ViewModels** (Class Library)
Contains API request/response models:
- `EmployeeViewModel`: API response model for employees
- `LeaveViewModel`: API response model for leaves with calculated total days
- `LeaveBalanceViewModel`: API response model for leave balances
- `CreateLeaveRequestViewModel`: API request model for creating leaves
- `ApiResponse<T>`: Generic wrapper for all API responses

### 3. **DataService** (Class Library)
Contains business logic and data access layer:
- `ILeaveDataService`: Interface defining all data operations
- `LeaveDataService`: Implementation using JSON file as database
- `Database`: Helper class for JSON deserialization

**Key Features:**
- Thread-safe JSON file operations using locks
- Automatic ID generation for new records
- LINQ-based filtering and querying
- Async/await pattern for all operations

### 4. **LeaveManagementSystem** (ASP.NET Core Web API)
Main web application with REST API endpoints:
- `EmployeesController`: Employee CRUD operations
- `LeavesController`: Leave request management
- `LeaveBalancesController`: Leave balance tracking

**Registered in Program.cs:**
- DataService dependency injection
- CORS policy for cross-origin requests
- OpenAPI documentation support

## Database

The system uses a JSON file (`Data/database.json`) as the database, containing:
- 5 sample employees
- 5 sample leave requests with various statuses
- 15 leave balance records (3 leave types per employee for 2025)

## API Endpoints

### Employees
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Leaves
- `GET /api/leaves` - Get all leaves
- `GET /api/leaves/{id}` - Get leave by ID
- `GET /api/leaves/employee/{employeeId}` - Get leaves for specific employee
- `GET /api/leaves/status/{status}` - Get leaves by status (Pending, Approved, Rejected)
- `POST /api/leaves` - Create new leave request
- `PUT /api/leaves/{id}` - Update leave status or details
- `DELETE /api/leaves/{id}` - Delete leave request

### Leave Balances
- `GET /api/leavebalances` - Get all leave balances
- `GET /api/leavebalances/{id}` - Get leave balance by ID
- `GET /api/leavebalances/employee/{employeeId}` - Get balances for employee
- `GET /api/leavebalances/employee/{employeeId}/year/{year}` - Get balances for employee in specific year
- `POST /api/leavebalances` - Create new leave balance
- `PUT /api/leavebalances/{id}` - Update leave balance
- `DELETE /api/leavebalances/{id}` - Delete leave balance

## API Response Format

All API responses follow this standard format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* response data */ }
}
```

### Example: Get All Employees
**Request:**
```
GET /api/employees
```

**Response:**
```json
{
  "success": true,
  "message": "Employees retrieved successfully",
  "data": [
    {
      "employeeId": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@company.com",
      "department": "Engineering",
      "joinDate": "2022-01-15T00:00:00"
    }
  ]
}
```

### Example: Create Leave Request
**Request:**
```
POST /api/leaves
Content-Type: application/json

{
  "employeeId": 1,
  "startDate": "2025-01-20T00:00:00",
  "endDate": "2025-01-22T00:00:00",
  "leaveType": "Annual",
  "reason": "Personal vacation"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Leave created successfully",
  "data": {
    "leaveId": 6,
    "employeeId": 1,
    "employeeName": "John Doe",
    "startDate": "2025-01-20T00:00:00",
    "endDate": "2025-01-22T00:00:00",
    "totalDays": 3,
    "leaveType": "Annual",
    "status": "Pending",
    "reason": "Personal vacation",
    "appliedDate": "2024-12-05T12:30:45.123Z"
  }
}
```

## Building and Running

### Prerequisites
- .NET 9.0 SDK

### Build
```powershell
cd LeaveManagementSystem
dotnet build
```

### Run
```powershell
dotnet run
```

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP)

### View OpenAPI Documentation
Navigate to `https://localhost:5001/openapi/v1.json` to view the OpenAPI specification.

## Data Persistence

The JSON database file is located at `LeaveManagementSystem/Data/database.json`. All CRUD operations automatically serialize changes back to this file with proper indentation for readability.

## Thread Safety

The DataService uses file-level locking to ensure thread-safe concurrent access to the JSON database file.

## Future Enhancements

1. Implement SQL Server database integration
2. Add authentication and authorization
3. Implement leave approval workflow
4. Add notification system for leave approvals
5. Implement leave balance calculations
6. Add audit logging
7. Create Angular/React frontend
