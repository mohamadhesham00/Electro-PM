# 📚 ELECTRO PM API - COMPLETE POSTMAN DOCUMENTATION & GUIDE

**Version**: 1.0.0  
**Last Updated**: January 2025  
**Status**: Production Ready ✅  
**Target Audience**: Frontend Developers, QA Engineers, API Consumers

---

## 📖 TABLE OF CONTENTS

1. [Quick Start](#quick-start)
2. [Authentication Flow](#authentication-flow)
3. [Data Types & Schemas](#data-types--schemas)
4. [Complete API Reference](#complete-api-reference)
5. [Environment Setup](#environment-setup)
6. [Request/Response Examples](#requestresponse-examples)
7. [Testing Workflows](#testing-workflows)
8. [Error Handling](#error-handling)
9. [Pro Tips & Best Practices](#pro-tips--best-practices)
10. [Postman Collection JSON](#postman-collection-json)
11. [Postman Environment JSON](#postman-environment-json)

---

## 🚀 QUICK START

### Prerequisites
- Postman (v11.0+) installed
- API running at `http://localhost:5000`
- Valid email and password for testing

### Setup in 3 Steps

**Step 1: Create Environment**
1. Open Postman
2. Click **Environments** → **Create New**
3. Name it: `Electro PM Development`
4. Add these variables:
   ```
   base_url = http://localhost:5000
   user_token = (empty - will be filled after login)
   admin_token = (empty - will be filled after admin login)
   project_id = (empty - will be filled after creating project)
   task_id = (empty - will be filled after creating task)
   ```
5. Save and select this environment

**Step 2: Register & Login**
1. Create a POST request to `{{base_url}}/api/auth/register`
2. Body (raw JSON):
   ```json
   {
     "email": "testuser@example.com",
     "password": "TestPass123"
   }
   ```
3. Send and verify 200 response
4. Login with same credentials at `{{base_url}}/api/auth/login`
5. Copy the token from response to `user_token` variable

**Step 3: Start Testing**
- All subsequent requests will use `Bearer {{user_token}}` in Authorization header
- Ready to test projects, tasks, and other endpoints!

---

## 🔐 AUTHENTICATION FLOW

### User Registration
```
POST /api/auth/register
Content-Type: application/json

Request:
{
  "email": "user@example.com",
  "password": "SecurePassword123"
}

Success Response (200 OK):
{
  "succeeded": true,
  "data": null,
  "message": "User registered successfully.",
  "errors": null
}

Error Response (400 Bad Request):
{
  "succeeded": false,
  "data": null,
  "message": "",
  "errors": ["Email already taken"]
}
```

### User Login
```
POST /api/auth/login
Content-Type: application/json

Request:
{
  "email": "user@example.com",
  "password": "SecurePassword123"
}

Success Response (200 OK):
{
  "succeeded": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDEiLCJlbWFpbCI6InVzZXJAZXhhbXBsZS5jb20iLCJyb2xlIjoiVXNlciIsIm5iZiI6MTczMzAwMDAwMCwiZXhwIjoxNzMzMDAzNjAwLCJpYXQiOjE3MzMwMDAwMDB9...",
    "email": "user@example.com"
  },
  "message": "User logged in successfully.",
  "errors": null
}

Error Response (401 Unauthorized):
{
  "succeeded": false,
  "data": null,
  "message": "",
  "errors": ["User not found", "Invalid password"]
}
```

### Token Usage
After login, use token in all authenticated requests:
```
Authorization: Bearer {token}
```

Token Details:
- Format: JWT (JSON Web Token)
- Expiration: Configured in settings (typically 1 hour)
- Contains: User ID, Email, Role, Expiration time
- Include in every request header as shown above

---

## 📊 DATA TYPES & SCHEMAS

### TypeScript Interfaces

#### User Object
```typescript
interface User {
  id: string;              // GUID (e.g., "550e8400-e29b-41d4-a716-446655440001")
  email: string;           // Email address (unique, required)
  password: string;        // BCrypt hashed password (never returned in responses)
  role: UserRole;          // "Admin" or "User"
}

type UserRole = "Admin" | "User";
```

#### Project Object
```typescript
interface Project {
  id: string;              // GUID (auto-generated)
  name: string;            // Project name (required, max 100 characters)
  description: string;     // Optional description (max 500 characters)
  createdAt: string;       // ISO 8601 datetime (e.g., "2025-01-15T10:30:00Z")
  ownerId: string;         // GUID of project owner/creator
}
```

#### Task Object
```typescript
interface Task {
  id: string;              // GUID (auto-generated)
  title: string;           // Task title (required, max 100 characters)
  description: string;     // Optional description (max 500 characters)
  status: TaskStatus;      // Current task status
  dueDate: string | null;  // ISO 8601 datetime (optional)
  priority: TaskPriority;  // Task priority level
  projectId: string;       // GUID of parent project (required)
}

type TaskStatus = "Pending" | "InProgress" | "Completed" | "OnHold" | "Cancelled";
type TaskPriority = "Low" | "Medium" | "High" | "Critical";
```

#### API Response Wrapper
```typescript
interface ApiResponse<T> {
  succeeded: boolean;      // true for success, false for error
  data: T | null;          // Response data (null if error)
  message: string;         // Human-readable message
  errors: string[] | null; // Array of error messages (null if success)
}

// Example Usage:
type LoginResponse = ApiResponse<{
  token: string;
  email: string;
}>;

type ProjectResponse = ApiResponse<Project>;

type ProjectListResponse = ApiResponse<Project[]>;
```

#### DTO Classes (Request Models)

**CreateProjectRequest**
```typescript
{
  name: string;           // Required, max 100 chars
  description: string;    // Optional, max 500 chars
}
```

**UpdateProjectRequest** (Partial)
```typescript
{
  name?: string;          // Optional, max 100 chars
  description?: string;   // Optional, max 500 chars
}
```

**CreateTaskRequest**
```typescript
{
  title: string;          // Required, max 100 chars
  description: string;    // Optional, max 500 chars
  status: TaskStatus;     // Required: "Pending" | "InProgress" | "Completed" | "OnHold" | "Cancelled"
  dueDate?: string;       // Optional ISO 8601 datetime
  priority: TaskPriority; // Required: "Low" | "Medium" | "High" | "Critical"
}
```

**UpdateTaskRequest** (Partial)
```typescript
{
  title?: string;         // Optional, max 100 chars
  description?: string;   // Optional, max 500 chars
  status?: TaskStatus;    // Optional
  dueDate?: string;       // Optional ISO 8601 datetime
  priority?: TaskPriority; // Optional
}
```

---

## 📋 COMPLETE API REFERENCE

### 🔐 AUTHENTICATION ENDPOINTS

#### POST /api/auth/register
**Description**: Register a new user account

**Authentication**: Not required

**Request**:
```json
{
  "email": "newuser@example.com",
  "password": "StrongPassword123"
}
```

**Curl**:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"newuser@example.com","password":"StrongPassword123"}'
```

**Validation Rules**:
- Email must be valid format and unique
- Password minimum 6 characters

**Status Codes**:
- 200: Registration successful
- 400: Invalid input or email already exists

---

#### POST /api/auth/login
**Description**: Authenticate user and receive JWT token

**Authentication**: Not required

**Request**:
```json
{
  "email": "user@example.com",
  "password": "StrongPassword123"
}
```

**Response (200)**:
```json
{
  "succeeded": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "user@example.com"
  },
  "message": "User logged in successfully.",
  "errors": null
}
```

**Curl**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"StrongPassword123"}'
```

**Status Codes**:
- 200: Login successful
- 401: Invalid credentials

---

### 📋 PROJECT ENDPOINTS

#### POST /api/projects
**Description**: Create a new project

**Authentication**: Required (Bearer token)

**Request**:
```json
{
  "name": "Q1 2025 Initiative",
  "description": "Strategic project for Q1 quarter"
}
```

**Response (200)**:
```json
{
  "succeeded": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000"
  },
  "message": "Project created successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X POST http://localhost:5000/api/projects \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Q1 2025 Initiative","description":"Strategic project"}'
```

---

#### GET /api/projects
**Description**: Get all projects for authenticated user

**Authentication**: Required (Bearer token)

**Response (200)**:
```json
{
  "succeeded": true,
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Q1 2025 Initiative",
      "description": "Strategic project",
      "createdAt": "2025-01-15T10:30:00Z",
      "ownerId": "550e8400-e29b-41d4-a716-446655440001"
    }
  ],
  "message": "Projects retrieved successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X GET http://localhost:5000/api/projects \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

#### GET /api/projects/{projectId}
**Description**: Get specific project by ID

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Project identifier

**Response (200)**:
```json
{
  "succeeded": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Q1 2025 Initiative",
    "description": "Strategic project",
    "createdAt": "2025-01-15T10:30:00Z",
    "ownerId": "550e8400-e29b-41d4-a716-446655440001"
  },
  "message": "Project retrieved successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X GET http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

#### PUT /api/projects/{projectId}
**Description**: Update project (supports partial updates)

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Project identifier

**Request** (update only name):
```json
{
  "name": "Updated Project Name"
}
```

**Request** (update both fields):
```json
{
  "name": "New Name",
  "description": "New description"
}
```

**Response (200)**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Project updated successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X PUT http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Name"}'
```

---

#### DELETE /api/projects/{projectId}
**Description**: Delete a project

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Project identifier

**Response (200)**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Project deleted successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X DELETE http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### ✅ TASK ENDPOINTS

#### POST /api/projects/{projectId}/tasks
**Description**: Create task in project

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Parent project identifier

**Request**:
```json
{
  "title": "Design Database Schema",
  "description": "Create normalized database design",
  "status": "Pending",
  "dueDate": "2025-06-30T23:59:59Z",
  "priority": "High"
}
```

**Valid Status Values**:
- `Pending` (default)
- `InProgress`
- `Completed`
- `OnHold`
- `Cancelled`

**Valid Priority Values**:
- `Low`
- `Medium`
- `High`
- `Critical`

**Response (200)**:
```json
{
  "succeeded": true,
  "data": {
    "id": "323e4567-e89b-12d3-a456-426614174000"
  },
  "message": "Task created successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X POST http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000/tasks \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title":"Design Database",
    "description":"Create normalized design",
    "status":"Pending",
    "priority":"High"
  }'
```

---

#### GET /api/projects/{projectId}/tasks
**Description**: Get all tasks in project

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Parent project identifier

**Response (200)**:
```json
{
  "succeeded": true,
  "data": [
    {
      "id": "323e4567-e89b-12d3-a456-426614174000",
      "title": "Design Database Schema",
      "description": "Create normalized database design",
      "status": "Pending",
      "dueDate": "2025-06-30T23:59:59Z",
      "priority": "High",
      "projectId": "123e4567-e89b-12d3-a456-426614174000"
    }
  ],
  "message": "Tasks retrieved successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X GET http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000/tasks \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

#### PATCH /api/projects/{projectId}/tasks/{taskId}
**Description**: Update task (supports partial updates)

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Parent project identifier
- `taskId` (GUID): Task identifier

**Request** (update status only):
```json
{
  "status": "InProgress"
}
```

**Request** (update multiple fields):
```json
{
  "status": "Completed",
  "priority": "Critical"
}
```

**Response (200)**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Task updated successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X PATCH http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000/tasks/323e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"status":"InProgress"}'
```

---

#### DELETE /api/projects/{projectId}/tasks/{taskId}
**Description**: Delete task from project

**Authentication**: Required (Bearer token)

**Path Parameters**:
- `projectId` (GUID): Parent project identifier
- `taskId` (GUID): Task identifier

**Response (200)**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Task deleted successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X DELETE http://localhost:5000/api/projects/123e4567-e89b-12d3-a456-426614174000/tasks/323e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### 👥 USER MANAGEMENT ENDPOINTS (Admin Only)

#### GET /api/users
**Description**: List all users (Admin only)

**Authentication**: Required - Admin role

**Response (200)**:
```json
{
  "succeeded": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "admin@electropm.com",
      "role": "Admin"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "email": "user@example.com",
      "role": "User"
    }
  ],
  "message": "Users retrieved successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer ADMIN_TOKEN"
```

---

#### DELETE /api/users/{userId}
**Description**: Delete user (Admin only, cannot delete self)

**Authentication**: Required - Admin role

**Path Parameters**:
- `userId` (GUID): User to delete

**Restrictions**:
- Cannot delete own account
- Admin role required

**Response (200)**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "User deleted successfully!",
  "errors": null
}
```

**Curl**:
```bash
curl -X DELETE http://localhost:5000/api/users/550e8400-e29b-41d4-a716-446655440001 \
  -H "Authorization: Bearer ADMIN_TOKEN"
```

---

## ⚙️ ENVIRONMENT SETUP

### Postman Environment Variables

Create a new Postman environment with these variables:

```json
{
  "name": "Electro PM Development",
  "values": [
    {
      "key": "base_url",
      "value": "http://localhost:5000",
      "enabled": true
    },
    {
      "key": "admin_email",
      "value": "admin@electropm.com",
      "enabled": true
    },
    {
      "key": "admin_password",
      "value": "Admin@123456",
      "enabled": true
    },
    {
      "key": "test_user_email",
      "value": "testuser@example.com",
      "enabled": true
    },
    {
      "key": "test_user_password",
      "value": "TestUser@123456",
      "enabled": true
    },
    {
      "key": "user_token",
      "value": "",
      "enabled": true
    },
    {
      "key": "admin_token",
      "value": "",
      "enabled": true
    },
    {
      "key": "project_id",
      "value": "",
      "enabled": true
    },
    {
      "key": "task_id",
      "value": "",
      "enabled": true
    },
    {
      "key": "user_id",
      "value": "",
      "enabled": true
    }
  ]
}
```

### How to Import Environment

1. **In Postman**:
   - Click **Environments** (left sidebar)
   - Click **Create New**
   - Name: "Electro PM Development"
   - Add all variables from above
   - Click **Save**

2. **Select Environment**:
   - Top-right dropdown
   - Select "Electro PM Development"
   - All requests will use variables from this environment

---

## 📝 REQUEST/RESPONSE EXAMPLES

### Complete Workflow Example

#### 1. Register New User
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "developer@company.com",
  "password": "DeveloperPass123!"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "User registered successfully.",
  "errors": null
}
```

#### 2. Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "developer@company.com",
  "password": "DeveloperPass123!"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDEiLCJlbWFpbCI6ImRldmVsb3BlckBjb21wYW55LmNvbSIsInJvbGUiOiJVc2VyIiwibmJmIjoxNzMzMDAwMDAwLCJleHAiOjE3MzMwMDM2MDAsImlhdCI6MTczMzAwMDAwMH0.SIGNATURE",
    "email": "developer@company.com"
  },
  "message": "User logged in successfully.",
  "errors": null
}
```

**👉 Copy token to `{{user_token}}`**

#### 3. Create Project
```http
POST http://localhost:5000/api/projects
Authorization: Bearer {{user_token}}
Content-Type: application/json

{
  "name": "Mobile App Development",
  "description": "Cross-platform mobile application for iOS and Android"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  },
  "message": "Project created successfully!",
  "errors": null
}
```

**👉 Copy ID to `{{project_id}}`**

#### 4. Create First Task
```http
POST http://localhost:5000/api/projects/{{project_id}}/tasks
Authorization: Bearer {{user_token}}
Content-Type: application/json

{
  "title": "Setup Development Environment",
  "description": "Install Node.js, React, and configure build tools",
  "status": "Pending",
  "dueDate": "2025-02-15T23:59:59Z",
  "priority": "High"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901"
  },
  "message": "Task created successfully!",
  "errors": null
}
```

**👉 Copy ID to `{{task_id}}`**

#### 5. Update Task Status
```http
PATCH http://localhost:5000/api/projects/{{project_id}}/tasks/{{task_id}}
Authorization: Bearer {{user_token}}
Content-Type: application/json

{
  "status": "InProgress"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Task updated successfully!",
  "errors": null
}
```

#### 6. Complete Task
```http
PATCH http://localhost:5000/api/projects/{{project_id}}/tasks/{{task_id}}
Authorization: Bearer {{user_token}}
Content-Type: application/json

{
  "status": "Completed",
  "priority": "Critical"
}
```

**Response**:
```json
{
  "succeeded": true,
  "data": null,
  "message": "Task updated successfully!",
  "errors": null
}
```

---

## 🧪 TESTING WORKFLOWS

### Workflow 1: Authentication Testing

```
1. POST /api/auth/register
   Email: test@example.com
   Password: TestPass123
   Expected: 200 OK

2. POST /api/auth/login
   Email: test@example.com
   Password: TestPass123
   Expected: 200 OK with token
   → Save token to {{user_token}}

3. POST /api/auth/login
   Email: admin@electropm.com
   Password: Admin@123456
   Expected: 200 OK with admin token
   → Save token to {{admin_token}}
```

### Workflow 2: Project Management

```
1. POST /api/projects
   Create project
   Expected: 200 OK with project ID
   → Save ID to {{project_id}}

2. GET /api/projects
   List all projects
   Expected: 200 OK with array of projects

3. GET /api/projects/{{project_id}}
   Get specific project
   Expected: 200 OK with project details

4. PUT /api/projects/{{project_id}}
   Update name only
   Expected: 200 OK

5. PUT /api/projects/{{project_id}}
   Update description only
   Expected: 200 OK

6. DELETE /api/projects/{{project_id}}
   Expected: 200 OK
```

### Workflow 3: Task Management

```
1. POST /api/projects/{{project_id}}/tasks
   Create task
   Expected: 200 OK with task ID
   → Save ID to {{task_id}}

2. GET /api/projects/{{project_id}}/tasks
   List all tasks
   Expected: 200 OK with array of tasks

3. PATCH /api/projects/{{project_id}}/tasks/{{task_id}}
   Update status
   Expected: 200 OK

4. PATCH /api/projects/{{project_id}}/tasks/{{task_id}}
   Update priority
   Expected: 200 OK

5. PATCH /api/projects/{{project_id}}/tasks/{{task_id}}
   Update multiple fields
   Expected: 200 OK

6. DELETE /api/projects/{{project_id}}/tasks/{{task_id}}
   Expected: 200 OK
```

### Workflow 4: Admin Operations

```
1. GET /api/users
   With {{admin_token}}
   Expected: 200 OK with user list

2. DELETE /api/users/{{user_id}}
   With {{admin_token}} (not own ID)
   Expected: 200 OK

3. DELETE /api/users/{{admin_id}}
   With {{admin_token}} (own ID)
   Expected: 400 error - cannot delete self
```

---

## ⚠️ ERROR HANDLING

### Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | OK | Request successful |
| 400 | Bad Request | Invalid input, validation error |
| 401 | Unauthorized | Missing or invalid token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Internal server error |

### Error Response Examples

**400 Bad Request - Validation Error**
```json
{
  "succeeded": false,
  "data": null,
  "message": "",
  "errors": [
    "Project name is required",
    "Description cannot exceed 500 characters"
  ]
}
```

**401 Unauthorized - Missing Token**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.3.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header was not provided"
}
```

**403 Forbidden - Insufficient Permission**
```json
{
  "succeeded": false,
  "data": null,
  "message": "",
  "errors": ["Only administrators can delete users."]
}
```

**404 Not Found**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Project not found or you don't have access to it."
}
```

### Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Invalid token | Login again and get new token |
| 403 Forbidden | Not admin | Use admin token for admin endpoints |
| 404 Not Found | Wrong ID | Verify ID matches created resource |
| 400 Bad Request | Invalid data | Check request body format and types |
| No response | API not running | Ensure API is running on `localhost:5000` |

---

## 💡 PRO TIPS & BEST PRACTICES

### 1. Auto-Save Tokens
Add test script to login endpoint:
```javascript
if (pm.response.code === 200) {
  var jsonData = pm.response.json();
  pm.environment.set("user_token", jsonData.data.token);
}
```

### 2. Auto-Save IDs
Add test script to create endpoints:
```javascript
if (pm.response.code === 200) {
  var jsonData = pm.response.json();
  pm.environment.set("project_id", jsonData.data.id);
}
```

### 3. Create Request Collections
- Group endpoints by feature
- Use folders: Auth, Projects, Tasks, Users
- Makes navigation easier

### 4. Use Pre-request Scripts
Validate data before sending:
```javascript
if (!pm.environment.get("user_token")) {
  console.error("user_token not set!");
}
```

### 5. Create Test Cases
```javascript
pm.test("Status code is 200", function() {
  pm.response.to.have.status(200);
});

pm.test("Response has succeeded flag", function() {
  var jsonData = pm.response.json();
  pm.expect(jsonData.succeeded).to.be.true;
});
```

### 6. Use Postman Environments
- Development: `http://localhost:5000`
- Testing: `http://test-api.example.com`
- Production: `http://api.example.com`

### 7. Document with Comments
Add comments in request description:
```
This endpoint creates a new project.
Only works with user token, not admin token.
Returns project ID in response data.
```

---

## 📦 POSTMAN COLLECTION JSON

Save this as `Electro-PM-API.postman_collection.json`:

```json
{
  "info": {
    "name": "Electro PM API",
    "description": "Complete API collection for Electro PM - Project Management System",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Authentication",
      "item": [
        {
          "name": "Register User",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"email\": \"user@example.com\",\n  \"password\": \"SecurePassword123\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/auth/register",
              "host": ["{{base_url}}"],
              "path": ["api", "auth", "register"]
            }
          }
        },
        {
          "name": "Login User",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"email\": \"user@example.com\",\n  \"password\": \"SecurePassword123\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/auth/login",
              "host": ["{{base_url}}"],
              "path": ["api", "auth", "login"]
            }
          }
        }
      ]
    },
    {
      "name": "Projects",
      "item": [
        {
          "name": "Create Project",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"name\": \"Q1 2025 Initiative\",\n  \"description\": \"Strategic project for Q1 quarter\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/projects",
              "host": ["{{base_url}}"],
              "path": ["api", "projects"]
            }
          }
        },
        {
          "name": "Get All Projects",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/projects",
              "host": ["{{base_url}}"],
              "path": ["api", "projects"]
            }
          }
        },
        {
          "name": "Get Project By ID",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}"]
            }
          }
        },
        {
          "name": "Update Project",
          "request": {
            "method": "PUT",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"name\": \"Updated Project Name\",\n  \"description\": \"Updated description\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}"]
            }
          }
        },
        {
          "name": "Delete Project",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}"]
            }
          }
        }
      ]
    },
    {
      "name": "Tasks",
      "item": [
        {
          "name": "Create Task",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"title\": \"Design Database Schema\",\n  \"description\": \"Create normalized database design\",\n  \"status\": \"Pending\",\n  \"dueDate\": \"2025-06-30T23:59:59Z\",\n  \"priority\": \"High\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}/tasks",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}", "tasks"]
            }
          }
        },
        {
          "name": "Get Tasks by Project",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}/tasks",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}", "tasks"]
            }
          }
        },
        {
          "name": "Update Task",
          "request": {
            "method": "PATCH",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"status\": \"InProgress\",\n  \"priority\": \"Critical\"\n}"
            },
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}/tasks/{{task_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}", "tasks", "{{task_id}}"]
            }
          }
        },
        {
          "name": "Delete Task",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{user_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/projects/{{project_id}}/tasks/{{task_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "projects", "{{project_id}}", "tasks", "{{task_id}}"]
            }
          }
        }
      ]
    },
    {
      "name": "Users (Admin Only)",
      "item": [
        {
          "name": "Get All Users",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{admin_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/users",
              "host": ["{{base_url}}"],
              "path": ["api", "users"]
            }
          }
        },
        {
          "name": "Delete User",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{admin_token}}"
              }
            ],
            "url": {
              "raw": "{{base_url}}/api/users/{{user_id}}",
              "host": ["{{base_url}}"],
              "path": ["api", "users", "{{user_id}}"]
            }
          }
        }
      ]
    }
  ],
  "variable": [
    {
      "key": "base_url",
      "value": "http://localhost:5000"
    },
    {
      "key": "user_token",
      "value": ""
    },
    {
      "key": "admin_token",
      "value": ""
    },
    {
      "key": "project_id",
      "value": ""
    },
    {
      "key": "task_id",
      "value": ""
    },
    {
      "key": "user_id",
      "value": ""
    }
  ]
}
```

---

## 🔧 POSTMAN ENVIRONMENT JSON

Save this as `Electro-PM-Environment.postman_environment.json`:

```json
{
  "id": "electropm-postman-env",
  "name": "Electro PM Development",
  "values": [
    {
      "key": "base_url",
      "value": "http://localhost:5000",
      "type": "string",
      "enabled": true
    },
    {
      "key": "admin_email",
      "value": "admin@electropm.com",
      "type": "string",
      "enabled": true
    },
    {
      "key": "admin_password",
      "value": "Admin@123456",
      "type": "string",
      "enabled": true
    },
    {
      "key": "test_user_email",
      "value": "testuser@example.com",
      "type": "string",
      "enabled": true
    },
    {
      "key": "test_user_password",
      "value": "TestUser@123456",
      "type": "string",
      "enabled": true
    },
    {
      "key": "user_token",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "admin_token",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "project_id",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "task_id",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "user_id",
      "value": "",
      "type": "string",
      "enabled": true
    }
  ],
  "_postman_variable_scope": "environment",
  "_postman_exported_at": "2025-01-15T10:00:00.000Z",
  "_postman_exported_using": "Postman/11.0.0"
}
```

---

## 📞 SUPPORT & RESOURCES

- **API Documentation**: http://localhost:5000/swagger
- **GitHub**: https://github.com/mohamadhesham00/Electro-PM
- **Postman Learning**: https://learning.postman.com/
- **REST Standards**: https://restfulapi.net/

---

**Status**: ✅ Production Ready  
**Last Updated**: January 2025  
**Version**: 1.0.0
