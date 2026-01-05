## Copilot Instructions for Medical First Aid Event & Shift Manager (Blazor)

### Project Overview
This Blazor application manages events and shifts for a medical first aid team. Users can create, view, and manage events, assign staff to shifts, and track attendance. The app should be visually attractive, easy to use, and fully responsive for desktop and mobile devices.

**Technology Stack:**
- .NET 10 (ASP.NET Core Blazor)
- Entity Framework Core with SQL Server
- ASP.NET Core Identity for authentication
- Bootstrap for UI
- CQRS architecture pattern

---

### Repository Structure

```
LotusPlanningApp/
├── Application/              # CQRS Commands and Queries
│   ├── Commands/            # Write operations (state modifications)
│   ├── Queries/             # Read operations (data retrieval)
│   ├── Common/              # Base CQRS interfaces
│   ├── DataAdapters/        # DTOs and mapping
│   └── DependencyInjection.cs
├── Infrastructure/          # Data access and external services
│   ├── Data/               # EF Core DbContext and migrations
│   ├── Commands/           # Command handlers
│   ├── Configuration/      # Configuration classes
│   └── *Repository.cs      # Repository implementations
├── Entities/               # Domain models (Event, Shift, Staff, etc.)
├── LotusPlanningApp/       # Main Blazor web app
│   ├── Components/         # Reusable Blazor components
│   ├── Controllers/        # API controllers
│   └── wwwroot/           # Static assets
├── CustomerPortal/         # Customer-facing portal
├── UITests/               # UI test suite
└── Program.cs             # Application entry point
```

---

### Design & UI Guidelines
- Use **Bootstrap** for all UI components to ensure a modern, responsive design.
- Prioritize **mobile usability**: test layouts on small screens, use offcanvas navigation, and touch-friendly controls.
- Apply a **clean, professional color scheme** (e.g., blue, white, and accent colors for alerts/status).
- Use **Bootstrap Cards** for event and shift listings.
- Include a **dashboard** with summary stats (upcoming events, active shifts, staff on duty).
- Use **modals** for creating/editing events and shifts.
- Navigation should be clear and simple (sidebar or top navbar).

---

### Features
- **Event Management**: Create, edit, delete, and view events (name, date, location, description).
- **Shift Scheduling**: Assign staff to shifts, view shift rosters, mark attendance.
- **Staff Directory**: List staff members, contact info, roles.
- **Responsive Calendar View**: Show events and shifts in a calendar (Bootstrap or third-party calendar component).
- **Notifications**: Alert staff of upcoming shifts/events (use Bootstrap alerts/toasts).

---

### Coding Standards
- Follow **SOLID principles** and clean architecture.
- Use **Blazor components** for reusable UI parts.
- Organize code into clear folders: `Components`, `Pages`, `Services`, `Models`.
- Use **dependency injection** for services (all services registered in `DependencyInjection.cs` or `Program.cs`).
- Write **async code** for data access and UI updates.
- Add **XML comments** for public methods/classes.
- Use **meaningful variable and method names**.
- Avoid code duplication; refactor common logic into helpers/services.
- Add **unit tests** for business logic.
- Use **records** for immutable data structures (Commands, Queries, DTOs).
- Prefer **nullable reference types** (`?`) for optional values.

### Build and Test Commands

**Building the application:**
```bash
dotnet build --configuration Release
```

**Running the application:**
```bash
dotnet run --project LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj
```

**Publishing the application:**
```bash
dotnet publish -c Release -o ./publish
```

**Running tests:**
```bash
dotnet test
```

**Creating a new migration:**
```bash
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

**Applying migrations:**
```bash
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

---

### CQRS Architecture (Command Query Responsibility Segregation)

The application uses CQRS to separate read operations (Queries) from write operations (Commands).

#### Structure

```
Application/
├── Commands/                    # Write operations (state modifications)
│   ├── Events/
│   ├── Shifts/
│   ├── Staff/
│   └── StaffAssignments/
├── Queries/                     # Read operations (data retrieval)
│   ├── Events/
│   ├── Shifts/
│   ├── Staff/
│   └── StaffAssignments/
├── Common/                      # Base interfaces
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   └── IQueryHandler.cs
└── ICommandDispatcher.cs        # Command dispatcher for Blazor components
```

#### Commands (Write Operations)

Commands represent intentions to change state. Each command:
- Inherits from `ICommand<TResult>`
- Contains all data needed for the operation
- Is processed by a corresponding `ICommandHandler<TCommand, TResult>`
- Should be immutable (use `record` type)

**Example:**
```csharp
// Command definition in Application/Commands/Events/
public record CreateEventCommand(EventDTO EventData) : ICommand<EventDTO>;

// Handler implementation in Infrastructure/Commands/Events/ or Application layer
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    private readonly IEventRepository _eventRepository;
    
    public CreateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<EventDTO> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        // Validate input
        // Apply business rules
        // Persist changes via repository
        // Return result
    }
}
```

#### Queries (Read Operations)

Queries represent requests for data. Each query:
- Inherits from `IQuery<TResult>`
- Is processed by a corresponding `IQueryHandler<TQuery, TResult>`
- Should not modify state (read-only)
- Should be immutable (use `record` type)

**Example:**
```csharp
// Query definition in Application/Queries/Events/
public record GetEventByIdQuery(int EventId) : IQuery<Event?>;

// Handler implementation in Application/Queries/Events/
public class GetEventByIdQueryHandler : IQueryHandler<GetEventByIdQuery, Event?>
{
    private readonly IEventRepository _eventRepository;
    
    public GetEventByIdQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<Event?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        return await _eventRepository.GetEventByIdAsync(query.EventId);
    }
}
```

#### Using CQRS in Blazor Components

**Direct injection of handlers (preferred):**

```csharp
@page "/events"
@inject GetAllEventsQueryHandler GetAllEventsHandler
@inject CreateEventCommandHandler CreateEventHandler

@code {
    private List<Event> events = new();
    
    protected override async Task OnInitializedAsync()
    {
        // Execute query
        var query = new GetAllEventsQuery();
        events = await GetAllEventsHandler.Handle(query, CancellationToken.None);
    }
    
    private async Task CreateEvent(EventDTO eventDto)
    {
        // Execute command
        var command = new CreateEventCommand(eventDto);
        var result = await CreateEventHandler.Handle(command, CancellationToken.None);
        
        // Refresh list
        await OnInitializedAsync();
    }
}
```

#### Best Practices

- **Commands** belong in `Application/Commands/<Domain>/` (e.g., `Application/Commands/Events/`)
- **Command Handlers** can be in Application or Infrastructure layer depending on complexity
- **Queries** belong in `Application/Queries/<Domain>/` (e.g., `Application/Queries/Events/`)
- **Query Handlers** typically belong in `Application/Queries/<Domain>/`
- Keep commands and queries **focused and single-purpose**
- Use **DTOs (Data Transfer Objects)** for data transfer between layers (defined in `Application/DataAdapters/`)
- **Register handlers** in `Application/DependencyInjection.cs` using the `AddApplicationLayer()` extension method
- **Always inject handlers directly** in Blazor components for best performance
- Handlers should use **repositories** for data access, not DbContext directly
- Use **CancellationToken** in all async operations for better resource management
- All handlers should handle exceptions gracefully and log errors

**Registering new handlers:**

```csharp
// In Application/DependencyInjection.cs
public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
{
    // Register command handler
    services.AddScoped<YourCommandHandler>();
    services.AddScoped(typeof(ICommandHandler<YourCommand, YourResult>), 
        typeof(YourCommandHandler));
    
    // Register query handler
    services.AddScoped<YourQueryHandler>();
    services.AddScoped(typeof(IQueryHandler<YourQuery, YourResult>), 
        typeof(YourQueryHandler));
    
    return services;
}
```

---

### Database & Entity Framework Core

#### Database Context
- Main DbContext: `Infrastructure/Data/ApplicationDbContext.cs`
- Uses SQL Server
- Connection string in `appsettings.json` under `DefaultConnection`

#### Migrations
**Creating migrations:**
```bash
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

**Applying migrations:**
```bash
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

**Rolling back migrations:**
```bash
dotnet ef database update <PreviousMigrationName> --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

#### Repository Pattern
- All data access goes through repositories (e.g., `IEventRepository`, `IStaffRepository`)
- Repository implementations are in `Infrastructure/*Repository.cs`
- Repository interfaces are in `Application/I*Repository.cs`
- **Never use DbContext directly in handlers or components**
- All repository methods should be async

**Example repository usage:**
```csharp
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    private readonly IEventRepository _eventRepository;
    
    public async Task<EventDTO> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = MapToEntity(command.EventData);
        var created = await _eventRepository.CreateEventAsync(eventEntity);
        return MapToDTO(created);
    }
}
```

#### Domain Models
- Located in `Entities/` folder
- Key entities: `Event`, `Shift`, `Staff`, `StaffAssignment`, `Customer`
- All entities have `CreatedAt` and `UpdatedAt` timestamps
- Use nullable reference types for optional properties

---

### Authentication & Authorization

#### User Management
- Uses **ASP.NET Core Identity** with `ApplicationUser`
- User model extended with: `FirstName`, `LastName`, `StaffId`, `IsApproved`, `ApprovedAt`
- Users must be approved by admin before they can log in (see `IsApproved` flag)
- Users can be linked to `Staff` members via `StaffId` foreign key

#### Authentication Flow
1. User registers at `/Account/Register` with first name, last name, email, password
2. System attempts auto-linking to Staff by matching email
3. User is created with `IsApproved = false`
4. Admin approves user via `/admin/user-approvals`
5. User can then log in

#### Authorization
- Check authentication state in components using `AuthenticationStateProvider`
- Use `[Authorize]` attribute for protected pages
- Use role-based authorization where needed

**Example:**
```csharp
@page "/admin/dashboard"
@attribute [Authorize(Roles = "Admin")]

<AuthorizeView Roles="Admin">
    <Authorized>
        <h3>Admin Dashboard</h3>
    </Authorized>
    <NotAuthorized>
        <p>You don't have permission to view this page.</p>
    </NotAuthorized>
</AuthorizeView>
```

---

### Services & Dependency Injection

#### Service Registration
- Application layer services: `Application/DependencyInjection.cs` using `AddApplicationLayer()`
- Infrastructure services: registered in `Program.cs`
- All services use **scoped** lifetime for request-level state

#### Key Services
- `IEmailService` - Email sending (configured in `appsettings.json`)
- `IShiftService` - Legacy shift operations (use CQRS handlers instead)
- `IStaffService` - Legacy staff operations (use CQRS handlers instead)
- `IStaffAssignmentService` - Legacy assignment operations (use CQRS handlers instead)
- `ICalendarService` - ICS calendar generation

#### Service Configuration
```csharp
// In Program.cs
builder.Services.AddApplicationLayer(); // Registers all CQRS handlers
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure options
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName));
```

---

### Bootstrap Integration
- Reference Bootstrap via CDN or include in `wwwroot/lib`.
- Use Bootstrap grid system for layouts.
- Prefer Bootstrap form controls, buttons, navbars, cards, modals, and alerts.
- Customize Bootstrap with a site-specific theme in `wwwroot/app.css`.

---

### Accessibility & Usability
- Ensure all interactive elements are keyboard accessible.
- Use ARIA attributes for modals, alerts, and navigation.
- Provide clear error messages and validation feedback.
- Test with screen readers and mobile devices.

---

### Example UI Components
- **Event Card**: Shows event details, edit/delete buttons.
- **Shift Modal**: Assign staff, select date/time, save/cancel.
- **Staff List**: Table or cards with contact info and role badges.
- **Dashboard**: Cards for stats, upcoming events, active shifts.

---

### Final Notes
- Keep UI simple, clean, and attractive.
- Use Bootstrap icons for visual cues.
- Document all major components and services.
- Ensure the app is easy to extend for future features (e.g., shift swap, event reports).

---

### Contribution Guidelines

#### Before Making Changes
1. **Understand the CQRS pattern** - Read `Application/README_CQRS.md`
2. **Check existing implementations** - Look at similar commands/queries for patterns
3. **Review domain models** - Understand entities in `Entities/` folder
4. **Check for existing migrations** - Don't duplicate database changes

#### Making Changes
1. **Create feature branches** from `main`
2. **Follow naming conventions**:
   - Commands: `<Verb><Entity>Command` (e.g., `CreateEventCommand`)
   - Queries: `Get<Entity><Criteria>Query` (e.g., `GetEventByIdQuery`)
   - Handlers: `<CommandOrQueryName>Handler`
3. **Add XML documentation** for public APIs
4. **Write tests** for business logic
5. **Update migrations** if changing database schema
6. **Register new handlers** in `DependencyInjection.cs`

#### Testing Changes
```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application locally
dotnet run --project LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj

# Apply migrations to local database
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp
```

#### Code Review Checklist
- [ ] Follows CQRS pattern
- [ ] Uses repositories instead of DbContext
- [ ] Includes XML documentation
- [ ] Properly registered in DI container
- [ ] Uses async/await correctly
- [ ] Handles exceptions gracefully
- [ ] Includes appropriate logging
- [ ] Mobile-responsive UI (if applicable)
- [ ] Follows Bootstrap patterns
- [ ] No hardcoded strings (use resources/constants)

---

### Common Patterns & Examples

#### Creating a New Domain Feature

1. **Define the domain model** (if needed):
```csharp
// In Entities/YourEntity.cs
public class YourEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

2. **Create repository interface**:
```csharp
// In Application/IYourEntityRepository.cs
public interface IYourEntityRepository
{
    Task<YourEntity?> GetByIdAsync(int id);
    Task<List<YourEntity>> GetAllAsync();
    Task<YourEntity> CreateAsync(YourEntity entity);
    Task<YourEntity> UpdateAsync(YourEntity entity);
    Task<bool> DeleteAsync(int id);
}
```

3. **Implement repository**:
```csharp
// In Infrastructure/YourEntityRepository.cs
public class YourEntityRepository : IYourEntityRepository
{
    private readonly ApplicationDbContext _context;
    
    public YourEntityRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<YourEntity?> GetByIdAsync(int id)
    {
        return await _context.YourEntities.FindAsync(id);
    }
    // ... other methods
}
```

4. **Create commands and queries**:
```csharp
// In Application/Commands/YourEntity/CreateYourEntityCommand.cs
public record CreateYourEntityCommand(YourEntityDTO Data) : ICommand<YourEntity>;

// In Application/Commands/YourEntity/CreateYourEntityCommandHandler.cs
public class CreateYourEntityCommandHandler : ICommandHandler<CreateYourEntityCommand, YourEntity>
{
    private readonly IYourEntityRepository _repository;
    
    public CreateYourEntityCommandHandler(IYourEntityRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<YourEntity> Handle(CreateYourEntityCommand command, CancellationToken cancellationToken)
    {
        var entity = new YourEntity 
        { 
            Name = command.Data.Name,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _repository.CreateAsync(entity);
    }
}
```

5. **Register in DI**:
```csharp
// In Application/DependencyInjection.cs
services.AddScoped<CreateYourEntityCommandHandler>();
services.AddScoped(typeof(ICommandHandler<CreateYourEntityCommand, YourEntity>), 
    typeof(CreateYourEntityCommandHandler));
```

6. **Use in Blazor component**:
```csharp
@inject CreateYourEntityCommandHandler CreateHandler

@code {
    private async Task Create(YourEntityDTO dto)
    {
        var command = new CreateYourEntityCommand(dto);
        var result = await CreateHandler.Handle(command, CancellationToken.None);
    }
}
```

---

### Troubleshooting

#### Common Issues

**"Handler not found" or "Cannot resolve service"**
- Check that handler is registered in `Application/DependencyInjection.cs`
- Verify `AddApplicationLayer()` is called in `Program.cs`

**Database migration errors**
- Ensure connection string is correct in `appsettings.json`
- Check that Infrastructure is set as the migrations project
- Verify startup project is LotusPlanningApp

**Authentication issues**
- Verify user has `IsApproved = true` in database
- Check role assignments if using role-based authorization

**Bootstrap styles not loading**
- Verify Bootstrap is referenced in `_Layout.cshtml` or `App.razor`
- Check browser console for 404 errors

---

### Resources

- **CQRS Documentation**: `Application/README_CQRS.md`
- **User Approval System**: `USER_APPROVAL_SYSTEM.md`
- **User-Staff Linking**: `USER_STAFF_LINKING.md`
- **Implementation Summary**: `IMPLEMENTATION_SUMMARY.md`
- **ICS Calendar Feature**: `ICS_CALENDAR_FEATURE.md`
- **Customer Portal**: `CUSTOMER_PORTAL_IMPLEMENTATION.md`

---

### Contact & Support

For questions or issues:
1. Check existing documentation files in repository root
2. Review similar implementations in the codebase
3. Consult the CQRS architecture documentation
4. Create an issue in the GitHub repository

