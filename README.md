# Lotus Planning App

A comprehensive medical first aid event and shift management system built with .NET 10 and Blazor. This application enables efficient scheduling, staff management, and coordination of medical first aid teams for events.

## 📋 Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Repository Structure](#repository-structure)
- [Getting Started](#getting-started)
- [Key Features](#key-features)
- [Database](#database)
- [Authentication & User Management](#authentication--user-management)
- [Documentation](#documentation)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

Lotus Planning App is a modern web application designed for medical first aid teams to manage events, shifts, and staff assignments. The system provides both an administrative interface for team coordinators and a customer portal for event organizers.

### Main Components

- **LotusPlanningApp** - Main administrative application for staff management
- **CustomerPortal** - Separate portal for customers to request and manage events
- **Application** - CQRS-based business logic layer
- **Infrastructure** - Data access and external service implementations
- **Entities** - Domain models and business entities

## 🛠️ Technology Stack

- **.NET 10** - Framework
- **ASP.NET Core Blazor** - Web framework (Server + WebAssembly hybrid)
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **ASP.NET Core Identity** - Authentication & Authorization
- **Bootstrap 5** - UI framework
- **CQRS Pattern** - Application architecture
- **Clean Architecture** - Overall design pattern

## 🏗️ Architecture

The application follows **Clean Architecture** principles with **CQRS (Command Query Responsibility Segregation)** pattern:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│              (Blazor Components & Pages)                     │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│                   Application Layer                          │
│  ┌──────────────────┐         ┌──────────────────────┐     │
│  │    Commands      │         │      Queries         │     │
│  │  (Write Ops)     │         │    (Read Ops)        │     │
│  └──────────────────┘         └──────────────────────┘     │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│            (Repositories, Services, DbContext)               │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│                    Domain Layer                              │
│                  (Entities, Models)                          │
└──────────────────────────────────────────────────────────────┘
```

### CQRS Pattern

The application uses CQRS to separate read operations (Queries) from write operations (Commands):

- **Commands** - State modifications (Create, Update, Delete)
- **Queries** - Data retrieval (GetAll, GetById, Search)
- **Handlers** - Business logic implementation for each command/query

For detailed CQRS documentation, see [Application/README_CQRS.md](Application/README_CQRS.md).

## 📁 Repository Structure

```
LotusPlanningApp/
├── Application/                 # CQRS business logic layer
│   ├── Commands/               # Write operations
│   │   ├── Events/
│   │   ├── Shifts/
│   │   ├── Staff/
│   │   └── StaffAssignments/
│   ├── Queries/                # Read operations
│   │   ├── Events/
│   │   ├── Shifts/
│   │   ├── Staff/
│   │   └── StaffAssignments/
│   ├── Common/                 # Base CQRS interfaces
│   ├── DataAdapters/           # DTOs and mapping
│   └── DependencyInjection.cs  # Service registration
│
├── Infrastructure/             # Data access and external services
│   ├── Data/                   # EF Core DbContext
│   ├── Migrations/             # Database migrations
│   ├── Commands/               # Command handler implementations
│   ├── Configuration/          # Configuration classes
│   └── *Repository.cs          # Repository implementations
│
├── Entities/                   # Domain models
│   ├── Event.cs
│   ├── Shift.cs
│   ├── Staff.cs
│   ├── StaffAssignment.cs
│   └── Customer.cs
│
├── LotusPlanningApp/           # Main Blazor web application
│   ├── LotusPlanningApp/       # Server project
│   │   ├── Components/         # Blazor components
│   │   │   ├── Account/        # Authentication
│   │   │   ├── Layout/         # Layout components
│   │   │   └── Pages/          # Page components
│   │   ├── Controllers/        # API controllers
│   │   ├── Services/           # App services
│   │   └── wwwroot/            # Static assets
│   └── LotusPlanningApp.Client/ # WebAssembly project
│
├── CustomerPortal/             # Customer-facing portal
│   ├── Components/             # Portal components
│   └── wwwroot/                # Portal assets
│
├── Application.Tests/          # Application layer tests
├── Infrastructure.Tests/       # Infrastructure tests
├── UITests/                    # UI tests
│
└── Documentation files (see below)
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet)
- A code editor (Visual Studio 2022, VS Code, or Rider)
- SQLite (included with .NET)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd LotusPlanningApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
   ```

4. **Run the application**
   ```bash
   dotnet run --project LotusPlanningApp/LotusPlanningApp
   ```

5. **Access the application**
   - Main App: `https://localhost:5001`
   - Default admin credentials are seeded automatically on first run
   - See [USER_APPROVAL_SYSTEM.md](USER_APPROVAL_SYSTEM.md#admin-user-seeding) for details

### Running CustomerPortal

```bash
dotnet run --project CustomerPortal
```

## ✨ Key Features

### Main Application (LotusPlanningApp)

- **Event Management** - Create, edit, and delete events
- **Shift Scheduling** - Organize shifts within events
- **Staff Management** - Maintain staff profiles with certifications
- **Staff Assignment** - Assign staff to shifts with availability checking
- **Check-in/Check-out** - Track staff attendance
- **User Approval System** - Admin approval required for new accounts
- **User-Staff Linking** - Automatic linking of user accounts to staff profiles
- **ICS Calendar Export** - Download shifts as calendar files
- **Dashboard** - Overview of upcoming events and active shifts
- **Mobile Responsive** - Works on all device sizes

### Customer Portal

- **Event Requests** - Customers can request new events
- **Event Management** - View and manage requested events
- **Cancellation Requests** - Request event cancellations
- **Customer Profiles** - Manage customer information
- **Separate Authentication** - Independent login system

## 🗄️ Database

The application uses **SQLite** as its database engine with **Entity Framework Core** for data access.

### Database Location
- Main App: `LotusPlanningApp/LotusPlanningApp/AppData/lotus.db`
- Both apps share the same database

### Migrations

**Create a new migration:**
```bash
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

**Apply migrations:**
```bash
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

**Rollback migration:**
```bash
dotnet ef database update <PreviousMigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

### Schema

Key entities:
- **Event** - Events that require medical first aid coverage
- **Shift** - Time slots within events
- **Staff** - Medical first aid team members
- **StaffAssignment** - Links staff to shifts
- **Customer** - Organizations requesting events
- **ApplicationUser** - User accounts (extends ASP.NET Identity)

## 🔐 Authentication & User Management

The application uses **ASP.NET Core Identity** for authentication and authorization.

### User Roles

- **Admin** - Full system access, user approval rights
- **Lotus** - Staff members, can manage events and shifts
- **Customer** - Customer portal users, can request events

### User Approval System

New user registrations require admin approval:

1. User registers with first name, last name, email, and password
2. Account created with `IsApproved = false`
3. User redirected to pending approval page
4. Admin reviews and approves in `/admin/user-approvals`
5. User can then log in

See [USER_APPROVAL_SYSTEM.md](USER_APPROVAL_SYSTEM.md) for details.

### User-Staff Linking

Users are automatically linked to staff members by matching email addresses during registration. This enables:
- Unified identity across user accounts and staff profiles
- Automatic profile synchronization
- Streamlined user experience

See [USER_STAFF_LINKING.md](USER_STAFF_LINKING.md) for details.

## 📚 Documentation

Comprehensive documentation is available in the repository:

### Architecture & Design
- [Application/README_CQRS.md](Application/README_CQRS.md) - CQRS pattern explanation
- [Application/ARCHITECTURE_DIAGRAM.md](Application/ARCHITECTURE_DIAGRAM.md) - Visual architecture reference
- [Application/MIGRATION_GUIDE.md](Application/MIGRATION_GUIDE.md) - Guide for migrating to CQRS
- [Application/MIGRATION_COMPLETE.md](Application/MIGRATION_COMPLETE.md) - Migration completion summary

### Features
- [USER_APPROVAL_SYSTEM.md](USER_APPROVAL_SYSTEM.md) - User approval workflow
- [USER_STAFF_LINKING.md](USER_STAFF_LINKING.md) - User-staff linking system
- [ICS_CALENDAR_FEATURE.md](ICS_CALENDAR_FEATURE.md) - Calendar export functionality
- [CUSTOMER_PORTAL_IMPLEMENTATION.md](CUSTOMER_PORTAL_IMPLEMENTATION.md) - Customer portal details
- [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) - Complete feature summary

### Setup & Configuration
- [ASPIRE_SETUP.md](ASPIRE_SETUP.md) - .NET Aspire configuration (if applicable)
- [LotusPlanningApp/EMAIL_SETUP.md](LotusPlanningApp/EMAIL_SETUP.md) - Email service configuration

## 🔨 Development

### Building the Application

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj

# Build for production
dotnet build --configuration Release
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Application.Tests
dotnet test Infrastructure.Tests
dotnet test UITests
```

### Code Structure Guidelines

- Follow **SOLID principles**
- Use **CQRS pattern** for new features
- Use **repositories** for data access (not DbContext directly)
- Write **async code** for all I/O operations
- Add **XML documentation** for public APIs
- Use **dependency injection** for services
- Follow **existing naming conventions**

### CQRS Development Pattern

**Creating a new feature:**

1. Define domain entity in `Entities/`
2. Create repository interface in `Application/`
3. Implement repository in `Infrastructure/`
4. Create commands in `Application/Commands/`
5. Create queries in `Application/Queries/`
6. Implement handlers
7. Register handlers in `Application/DependencyInjection.cs`
8. Use handlers in Blazor components

Example:
```csharp
// In Blazor component
@inject CreateEventCommandHandler CreateHandler

private async Task CreateEvent(EventDTO dto)
{
    var command = new CreateEventCommand(dto);
    var result = await CreateHandler.Handle(command, CancellationToken.None);
}
```

## 🤝 Contributing

### Coding Standards

- Use **C# naming conventions**
- Follow **existing patterns** in the codebase
- Write **unit tests** for business logic
- Update **documentation** for new features
- Use **meaningful commit messages**
- Keep **pull requests focused** and small

### Pull Request Process

1. Create a feature branch from `main`
2. Make your changes
3. Run tests and ensure they pass
4. Update documentation
5. Submit pull request with clear description
6. Address review feedback

### Testing Guidelines

- Write unit tests for command/query handlers
- Test edge cases and error conditions
- Use mocking for dependencies
- Maintain test coverage

## 📄 License

This project is licensed under the terms specified in [LICENSE.txt](LICENSE.txt).

## 🙏 Acknowledgments

Built with:
- ASP.NET Core Blazor
- Entity Framework Core
- Bootstrap
- And many other open-source libraries

---

**Note**: This is an active development project. For questions or issues, please open an issue on GitHub.
