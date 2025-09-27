# HomeApplianceCRM

This solution is structured using Clean Architecture principles (without CQRS) and consists of the following layers:
- Domain
- Application
- Infrastructure
- API

## Features
- ASP.NET Core 8 Web API
- Entity Framework Core with SQL Server
- Serilog for structured logging
- JWT authentication and role-based authorization
- Roles: Admin, Manager, CCO, Technician, Storekeeper, Dealer, Customer (future)
- Two-Factor Authentication for Admins
- Swagger (OpenAPI) documentation

## Getting Started

1. Ensure you have the .NET 8 SDK installed.
2. Update the connection string in `appsettings.json` under the API project.
3. Run database migrations from the Infrastructure project.
4. Launch the API project.

## Development
- All business logic should reside in the Application and Domain layers.
- Infrastructure handles data access and external integrations.
- API is the entry point for HTTP requests.

## Extensibility
- The solution is designed to easily add new roles (e.g., Customer) and features.

---

For more details, see the documentation in each project folder.
