# Lotus Planning App

A comprehensive medical first aid event and shift management application built with Blazor and .NET 10.

## Overview

Lotus Planning App is a Blazor-based web application designed for medical first aid teams to efficiently manage events, schedule shifts, and coordinate staff assignments. The application provides an intuitive interface for creating and managing events, assigning staff to shifts, tracking attendance, and maintaining staff information.

## Features

- **Event Management**: Create, edit, delete, and view events with details like name, date, location, and description
- **Shift Scheduling**: Create shifts, assign staff members, and manage shift rosters
- **Staff Directory**: Maintain a comprehensive directory of staff members with contact information and roles
- **Calendar View**: Visualize events and shifts in an easy-to-use calendar interface
- **Staff Assignments**: Track staff assignments and attendance (check-in/check-out)
- **Customer Portal**: Dedicated portal for customers to view and manage their events
- **User Authentication**: Secure authentication system with user approval workflow
- **Email Notifications**: Automated email notifications for shifts and events
- **ICS Calendar Export**: Export events and shifts to calendar applications

## Technology Stack

- **.NET 10** - Latest .NET framework
- **ASP.NET Core Blazor** - Modern web UI framework
- **Entity Framework Core** - ORM for database access
- **SQL Server** - Relational database
- **ASP.NET Core Identity** - Authentication and authorization
- **Bootstrap 5** - Responsive UI framework
- **CQRS Pattern** - Command Query Responsibility Segregation architecture

## Architecture

The application follows clean architecture principles with CQRS pattern:

- **Application Layer**: Contains CQRS commands, queries, and their handlers
- **Infrastructure Layer**: Implements data access, repositories, and external services
- **Entities Layer**: Domain models and business logic
- **LotusPlanningApp**: Main Blazor web application
- **CustomerPortal**: Customer-facing web portal

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) (or SQL Server Express/LocalDB)
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider recommended)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/JohanSmarius/LotusPlanningApp.git
   cd LotusPlanningApp
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LotusPlanningDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
   ```

4. Run the application:
   ```bash
   dotnet run --project LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj
   ```

5. Open your browser and navigate to `https://localhost:7001` (or the URL shown in the console)

## Building and Testing

### Build the application
```bash
dotnet build --configuration Release
```

### Run tests
```bash
dotnet test
```

### Publish the application
```bash
dotnet publish -c Release -o ./publish
```

## Development

### Project Structure

```
LotusPlanningApp/
├── Application/              # CQRS Commands and Queries
├── Application.Tests/        # Application layer tests
├── Infrastructure/           # Data access and external services
├── Infrastructure.Tests/     # Infrastructure layer tests
├── Entities/                 # Domain models
├── LotusPlanningApp/         # Main Blazor web app
├── CustomerPortal/           # Customer-facing portal
├── UITests/                  # UI test suite
└── Services/                 # Shared services
```

### Key Documentation

- [CQRS Architecture](Application/README_CQRS.md) - Detailed CQRS implementation guide
- [Architecture Diagram](Application/ARCHITECTURE_DIAGRAM.md) - Visual architecture overview
- [Migration Guide](Application/MIGRATION_GUIDE.md) - Service layer to CQRS migration
- [User Approval System](USER_APPROVAL_SYSTEM.md) - User registration and approval workflow
- [User-Staff Linking](USER_STAFF_LINKING.md) - Linking users to staff records
- [ICS Calendar Feature](ICS_CALENDAR_FEATURE.md) - Calendar export functionality
- [Customer Portal](CUSTOMER_PORTAL_IMPLEMENTATION.md) - Customer portal implementation
- [Email Setup](LotusPlanningApp/EMAIL_SETUP.md) - Email configuration guide

### Contributing

We welcome contributions! Please see our [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

#### Code Standards

- Follow SOLID principles and clean architecture
- Use async/await for all I/O operations
- Add XML documentation for public APIs
- Write unit tests for business logic
- Use CQRS pattern for new features (Commands for writes, Queries for reads)
- Follow repository pattern for data access
- Use nullable reference types

#### Development Workflow

1. Create a feature branch from `main`
2. Make your changes following the coding standards
3. Write tests for your changes
4. Ensure all tests pass: `dotnet test`
5. Build the solution: `dotnet build`
6. Submit a pull request

## Database Migrations

### Create a new migration
```bash
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

### Apply migrations
```bash
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

### Rollback to a previous migration
```bash
dotnet ef database update <PreviousMigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

## Configuration

### Email Settings

Configure SMTP settings in `appsettings.json`:

```json
{
  "EmailOptions": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@example.com",
    "SmtpPassword": "your-password",
    "FromEmail": "noreply@example.com",
    "FromName": "Lotus Planning"
  }
}
```

See [Email Setup Guide](LotusPlanningApp/EMAIL_SETUP.md) for detailed configuration.

### User Approval

New users must be approved by an administrator before they can log in. Administrators can approve users at `/admin/user-approvals`.

## Troubleshooting

### Common Issues

**"Cannot connect to database"**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database migrations are applied

**"Handler not found" or "Cannot resolve service"**
- Check that handler is registered in `Application/DependencyInjection.cs`
- Verify `AddApplicationLayer()` is called in `Program.cs`

**Build errors after pulling latest changes**
- Clean solution: `dotnet clean`
- Restore packages: `dotnet restore`
- Rebuild: `dotnet build`

For more troubleshooting tips, see the [Copilot Instructions](.github/copilot-instructions.md#troubleshooting).

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Support

For questions, issues, or feature requests:
1. Check existing documentation in the repository
2. Review the [CQRS architecture guide](Application/README_CQRS.md)
3. Create an issue in the GitHub repository

## Acknowledgments

Built with modern .NET technologies and best practices for maintainability, scalability, and developer experience.
