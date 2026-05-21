# Electro-PM

Project & Task Management API built with ASP.NET Core 9, Clean Architecture, EF Core, SQL Server, and JWT authentication.

## What this project includes

- User registration and login with JWT
- Project CRUD for authenticated users
- Task CRUD inside projects
- Admin-only user listing and deletion
- Global exception handling
- FluentValidation + MediatR pipeline
- Generic API response wrapper
- Swagger/OpenAPI documentation
- EF Core migrations and SQL Server support
- Docker Compose setup
- Unit tests

## Architecture

- `API` - controllers, middleware, Swagger, request pipeline
- `Application` - CQRS handlers, validators, DTOs, response models
- `Infrastructure` - EF Core, repositories, JWT token service, migrations
- `Domain` - entities, enums, repository contracts, exceptions
- `UnitTests` - xUnit tests for features and validators

## Requirements

- .NET 9 SDK
- SQL Server 2022 or Docker

## Run locally

### Option 1: Docker Compose

1. From the repository root:
   ```bash
   docker compose up --build
   ```
2. Open Swagger:
   - `http://localhost:8080/swagger`

Docker Compose starts:
- SQL Server on `1433`
- API on `8080`

### Option 2: Run with .NET

1. Start a SQL Server instance locally.
2. Update `API/appsettings.json` or set environment variables for:
   - `ConnectionStrings__DefaultConnection`
   - `JwtSettings__Key`
   - `JwtSettings__Issuer`
   - `JwtSettings__Audience`
   - `JwtSettings__ExpirationHours`
3. Apply migrations:
   ```bash
   dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj
   ```
4. Run the API:
   ```bash
   dotnet run --project API/API.csproj
   ```
5. Open Swagger:
   - `https://localhost:7106/swagger`
   - `http://localhost:5273/swagger`

## Test

```bash
dotnet test Electro-PM.slnx
```

## Database migrations

Migration files are in `Infrastructure/Migrations`.

## API documentation

- Swagger is available in development at `/swagger`
- Request examples are also documented in `docs/POSTMAN_COMPLETE_GUIDE.md`

## Seeded login users

The database seeds two users you can use right away:

- Admin
  ```json
  {
    "email": "admin@example.com",
    "password": "Strong@123"
  }
  ```
- User
  ```json
  {
    "email": "user@example.com",
    "password": "Strong@123"
  }
  ```

## Achievements

- Implemented JWT authentication for register/login
- Built project and task management endpoints scoped to authenticated users
- Added admin-only user management endpoints
- Used Clean Architecture with clear layer separation
- Added CQRS handlers with MediatR
- Added validation with FluentValidation
- Added a consistent `ApiResponse<T>` wrapper
- Added global exception handling and standardized error responses
- Added EF Core migrations and SQL Server persistence
- Added Swagger documentation and Docker support
- Added unit tests for core application behavior
