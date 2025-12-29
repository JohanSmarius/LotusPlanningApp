# CQRS Architecture Overview

## System Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        Presentation Layer                        │
│                     (Blazor Components)                          │
└───────────────┬──────────────────────────────┬──────────────────┘
                │                              │
                │ Inject Handlers              │
                ▼                              ▼
┌───────────────────────────┐    ┌────────────────────────────┐
│     Command Handlers      │    │     Query Handlers         │
│   (Write Operations)      │    │   (Read Operations)        │
├───────────────────────────┤    ├────────────────────────────┤
│ - Validate input          │    │ - Retrieve data            │
│ - Apply business rules    │    │ - No side effects          │
│ - Call domain services    │    │ - Return DTOs/Entities     │
│ - Send notifications      │    │ - Fast reads               │
│ - Persist changes         │    │                            │
└───────────┬───────────────┘    └────────────┬───────────────┘
            │                                  │
            │ Uses                            │ Uses
            ▼                                  ▼
┌──────────────────────────────────────────────────────────────┐
│                   Repository Interfaces                       │
│         (IEventRepository, IShiftRepository, etc.)            │
└───────────────────────────┬──────────────────────────────────┘
                            │
                            │ Implemented by
                            ▼
┌──────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                        │
│              (EventRepository, ShiftRepository)               │
└───────────────────────────┬──────────────────────────────────┘
                            │
                            │ Accesses
                            ▼
┌──────────────────────────────────────────────────────────────┐
│                        Database                               │
│                   (SQLite via EF Core)                        │
└──────────────────────────────────────────────────────────────┘
```

## Command Flow Example: Create Event

```
User clicks          Component calls          Handler processes       Repository saves
"Create Event"  →   CreateEventCommand   →   Business Logic      →   To Database
                    ┌─────────────────┐      ┌─────────────────┐    ┌─────────────┐
                    │ EventDTO data   │  →   │ Validate dates  │ →  │ INSERT INTO │
                    │ (name, date,    │      │ Create entity   │    │ Events      │
                    │  location...)   │      │ Add default     │    │ ...         │
                    └─────────────────┘      │ shift           │    └─────────────┘
                                             │ Log operation   │
                                             │ Return DTO      │
                                             └─────────────────┘
                                                    ↓
                                             Result returned
                                             to component
```

## Query Flow Example: Get All Events

```
Component loads  →  Component calls      →  Handler queries    →  Repository reads
                    GetAllEventsQuery       repository             from Database
                    ┌─────────────────┐    ┌─────────────────┐  ┌─────────────┐
                    │ No parameters   │ →  │ Call repo       │→ │ SELECT *    │
                    │ (just a marker) │    │ Return events   │  │ FROM Events │
                    └─────────────────┘    └─────────────────┘  └─────────────┘
                                                   ↓
                                            List<Event>
                                            returned to UI
```

## Directory Structure with File Counts

```
Application/
│
├── Commands/                           # 24 files
│   ├── Events/                         # 6 files
│   │   ├── CreateEventCommand.cs
│   │   ├── CreateEventCommandHandler.cs
│   │   ├── UpdateEventCommand.cs
│   │   ├── UpdateEventCommandHandler.cs
│   │   ├── DeleteEventCommand.cs
│   │   └── DeleteEventCommandHandler.cs
│   │
│   ├── Shifts/                         # 6 files
│   │   ├── CreateShiftCommand.cs
│   │   ├── CreateShiftCommandHandler.cs
│   │   ├── UpdateShiftCommand.cs
│   │   ├── UpdateShiftCommandHandler.cs
│   │   ├── DeleteShiftCommand.cs
│   │   └── DeleteShiftCommandHandler.cs
│   │
│   ├── Staff/                          # 6 files
│   │   ├── CreateStaffCommand.cs
│   │   ├── CreateStaffCommandHandler.cs
│   │   ├── UpdateStaffCommand.cs
│   │   ├── UpdateStaffCommandHandler.cs
│   │   ├── DeleteStaffCommand.cs
│   │   └── DeleteStaffCommandHandler.cs
│   │
│   └── StaffAssignments/               # 6 files
│       ├── CreateStaffAssignmentCommand.cs
│       ├── CreateStaffAssignmentCommandHandler.cs
│       ├── CheckInStaffCommand.cs
│       ├── CheckInStaffCommandHandler.cs
│       ├── CheckOutStaffCommand.cs
│       └── CheckOutStaffCommandHandler.cs
│
├── Queries/                            # 24 files
│   ├── Events/                         # 8 files
│   │   ├── GetAllEventsQuery.cs
│   │   ├── GetAllEventsQueryHandler.cs
│   │   ├── GetEventByIdQuery.cs
│   │   ├── GetEventByIdQueryHandler.cs
│   │   ├── GetUpcomingEventsQuery.cs
│   │   ├── GetUpcomingEventsQueryHandler.cs
│   │   ├── GetEventsByDateRangeQuery.cs
│   │   └── GetEventsByDateRangeQueryHandler.cs
│   │
│   ├── Shifts/                         # 8 files
│   │   ├── GetAllShiftsQuery.cs
│   │   ├── GetAllShiftsQueryHandler.cs
│   │   ├── GetShiftByIdQuery.cs
│   │   ├── GetShiftByIdQueryHandler.cs
│   │   ├── GetShiftsByEventIdQuery.cs
│   │   ├── GetShiftsByEventIdQueryHandler.cs
│   │   ├── GetUpcomingShiftsQuery.cs
│   │   └── GetUpcomingShiftsQueryHandler.cs
│   │
│   ├── Staff/                          # 6 files
│   │   ├── GetAllStaffQuery.cs
│   │   ├── GetAllStaffQueryHandler.cs
│   │   ├── GetStaffByIdQuery.cs
│   │   ├── GetStaffByIdQueryHandler.cs
│   │   ├── GetActiveStaffQuery.cs
│   │   └── GetActiveStaffQueryHandler.cs
│   │
│   └── StaffAssignments/               # 6 files
│       ├── GetAllAssignmentsQuery.cs
│       ├── GetAllAssignmentsQueryHandler.cs
│       ├── GetAssignmentsByShiftIdQuery.cs
│       ├── GetAssignmentsByShiftIdQueryHandler.cs
│       ├── GetAssignmentsByStaffIdQuery.cs
│       └── GetAssignmentsByStaffIdQueryHandler.cs
│
├── Common/                             # 4 files
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   └── IQueryHandler.cs
│
├── DataAdapters/                       # Existing DTOs and mappers
│
├── DependencyInjection.cs              # Service registration
│
├── README_CQRS.md                      # Architecture documentation
├── MIGRATION_GUIDE.md                  # Migration guide
├── ARCHITECTURE_DIAGRAM.md             # This file
│
└── [Legacy files]                      # Maintained for compatibility
    ├── CreateEventUseCase.cs           # Now wraps CreateEventCommandHandler
    ├── UpdateEventUseCase.cs           # Now wraps UpdateEventCommandHandler
    ├── EventService.cs                 # Now wraps UpdateEventCommandHandler
    └── ...
```

## Component Interaction Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                      Blazor Component                            │
│  @inject CreateEventCommandHandler CreateHandler                │
│  @inject GetAllEventsQueryHandler GetAllHandler                 │
└───────┬─────────────────────────────────────────┬───────────────┘
        │                                         │
        │ Create Event                           │ Load Events
        ▼                                         ▼
┌──────────────────────┐              ┌─────────────────────────┐
│ CreateEventCommand   │              │ GetAllEventsQuery       │
├──────────────────────┤              ├─────────────────────────┤
│ EventDTO EventData   │              │ (No parameters)         │
└──────────┬───────────┘              └──────────┬──────────────┘
           │                                     │
           ▼                                     ▼
┌──────────────────────────────┐    ┌──────────────────────────┐
│ CreateEventCommandHandler    │    │ GetAllEventsQueryHandler │
├──────────────────────────────┤    ├──────────────────────────┤
│ 1. Validate EventData        │    │ 1. Call repository       │
│ 2. Convert DTO to Entity     │    │ 2. Return List<Event>    │
│ 3. Set default properties    │    └──────────────────────────┘
│ 4. Add default shift         │
│ 5. Call repository.Create    │
│ 6. Log operation             │
│ 7. Convert Entity to DTO     │
│ 8. Return EventDTO           │
└──────────────────────────────┘
           │
           ▼
┌──────────────────────┐
│ IEventRepository     │
├──────────────────────┤
│ CreateEventAsync()   │
│ GetAllEventsAsync()  │
│ ...                  │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ EventRepository      │
│ (Infrastructure)     │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ Database (SQLite)    │
└──────────────────────┘
```

## Key Design Decisions

### 1. Commands and Queries are Records
```csharp
public record CreateEventCommand(EventDTO EventData) : ICommand<EventDTO>;
```
- Immutable by default
- Value-based equality
- Concise syntax
- Perfect for data transfer

### 2. Handlers are Classes
```csharp
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    // Business logic here
}
```
- Can maintain state (logger, repositories)
- Easy to inject dependencies
- Testable in isolation

### 3. Separation of Concerns
```
Commands (Commands/)     → State modification
Queries (Queries/)       → Data retrieval
Common (Common/)         → Shared contracts
DataAdapters (Legacy)    → DTOs and mappers
```

### 4. Backward Compatibility
```csharp
// Old code still works
@inject ICreateEventUseCase CreateUseCase

// New code uses CQRS
@inject CreateEventCommandHandler CreateHandler
```

## Summary Statistics

| Category | Count | Purpose |
|----------|-------|---------|
| Commands | 12 | State modifications (Create, Update, Delete, CheckIn, CheckOut) |
| Queries | 12 | Data retrieval (GetAll, GetById, GetUpcoming, etc.) |
| Handlers | 24 | Business logic implementation |
| Base Interfaces | 4 | ICommand, ICommandHandler, IQuery, IQueryHandler |
| Legacy Wrappers | 3 | Backward compatibility |
| **Total New Files** | **55+** | Complete CQRS implementation |

## Benefits Achieved

✅ **Clear Separation**: Commands vs Queries
✅ **Single Responsibility**: One handler, one operation
✅ **Testability**: Mock handlers easily
✅ **Maintainability**: Easy to find and modify code
✅ **Scalability**: Can optimize reads and writes separately
✅ **Type Safety**: Strongly typed commands and queries
✅ **Domain Alignment**: Operations match business language
✅ **Backward Compatible**: Existing code continues to work
