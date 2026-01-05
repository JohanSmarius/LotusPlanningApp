# Application Layer - CQRS Architecture

This document describes the CQRS (Command Query Responsibility Segregation) architecture implemented in the Application layer.

## Overview

The Application layer now follows the **CQRS pattern**, which separates read operations (Queries) from write operations (Commands). This provides:

- **Clear separation of concerns**: Commands modify state, Queries retrieve data
- **Better testability**: Each handler has a single responsibility
- **Improved maintainability**: Easy to locate and modify specific operations
- **Scalability**: Commands and Queries can be optimized independently
- **Single Responsibility Principle**: Each handler does one thing

## Architecture Structure

```
Application/
├── Commands/                      # Write operations (state modifications)
│   ├── Events/
│   │   ├── CreateEventCommand.cs
│   │   ├── CreateEventCommandHandler.cs
│   │   ├── UpdateEventCommand.cs
│   │   ├── UpdateEventCommandHandler.cs
│   │   ├── DeleteEventCommand.cs
│   │   └── DeleteEventCommandHandler.cs
│   ├── Shifts/
│   │   ├── CreateShiftCommand.cs
│   │   ├── CreateShiftCommandHandler.cs
│   │   ├── UpdateShiftCommand.cs
│   │   ├── UpdateShiftCommandHandler.cs
│   │   ├── DeleteShiftCommand.cs
│   │   └── DeleteShiftCommandHandler.cs
│   ├── Staff/
│   │   ├── CreateStaffCommand.cs
│   │   ├── CreateStaffCommandHandler.cs
│   │   ├── UpdateStaffCommand.cs
│   │   ├── UpdateStaffCommandHandler.cs
│   │   ├── DeleteStaffCommand.cs
│   │   └── DeleteStaffCommandHandler.cs
│   └── StaffAssignments/
│       ├── CreateStaffAssignmentCommand.cs
│       ├── CreateStaffAssignmentCommandHandler.cs
│       ├── CheckInStaffCommand.cs
│       ├── CheckInStaffCommandHandler.cs
│       ├── CheckOutStaffCommand.cs
│       └── CheckOutStaffCommandHandler.cs
├── Queries/                       # Read operations (data retrieval)
│   ├── Events/
│   │   ├── GetAllEventsQuery.cs
│   │   ├── GetAllEventsQueryHandler.cs
│   │   ├── GetEventByIdQuery.cs
│   │   ├── GetEventByIdQueryHandler.cs
│   │   ├── GetUpcomingEventsQuery.cs
│   │   ├── GetUpcomingEventsQueryHandler.cs
│   │   ├── GetEventsByDateRangeQuery.cs
│   │   └── GetEventsByDateRangeQueryHandler.cs
│   ├── Shifts/
│   ├── Staff/
│   └── StaffAssignments/
├── Common/                        # Base interfaces
│   ├── ICommand.cs               # Marker interface for commands
│   ├── ICommandHandler.cs        # Handler interface for commands
│   ├── IQuery.cs                 # Marker interface for queries
│   └── IQueryHandler.cs          # Handler interface for queries
├── DataAdapters/                 # DTOs and mappers
├── DependencyInjection.cs        # Service registration
└── [Legacy files maintained for backward compatibility]
```

## Core Concepts

### Commands

Commands represent **intentions to change state**. They encapsulate all the data needed to perform an operation.

**Example:**
```csharp
public record CreateEventCommand(EventDTO EventData) : ICommand<EventDTO>;
```

### Command Handlers

Command handlers contain the **business logic** for processing commands. Each handler:
- Validates input
- Applies business rules
- Interacts with repositories
- Returns a result

**Example:**
```csharp
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    public async Task<EventDTO> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        // Validate, process, persist
        return result;
    }
}
```

### Queries

Queries represent **requests for data**. They never modify state.

**Example:**
```csharp
public record GetEventByIdQuery(int EventId) : IQuery<Event?>;
```

### Query Handlers

Query handlers retrieve data from repositories without side effects.

**Example:**
```csharp
public class GetEventByIdQueryHandler : IQueryHandler<GetEventByIdQuery, Event?>
{
    public async Task<Event?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetEventByIdAsync(query.EventId);
    }
}
```

## Usage Examples

### Using Commands in Blazor Components

```csharp
@inject CreateEventCommandHandler CreateEventHandler

private async Task CreateEvent()
{
    var command = new CreateEventCommand(eventDto);
    var result = await CreateEventHandler.Handle(command);
}
```

### Using Queries in Blazor Components

```csharp
@inject GetAllEventsQueryHandler GetAllEventsHandler

protected override async Task OnInitializedAsync()
{
    var query = new GetAllEventsQuery();
    events = await GetAllEventsHandler.Handle(query);
}
```

### Using Legacy Services (Backward Compatible)

The old interfaces still work:

```csharp
@inject ICreateEventUseCase CreateEventUseCase

private async Task CreateEvent()
{
    var result = await CreateEventUseCase.Execute(eventDto);
}
```

## Dependency Injection

All handlers are registered automatically using the extension method:

```csharp
// In Program.cs
builder.Services.AddApplicationLayer();
```

This registers:
- All command handlers
- All query handlers
- Legacy service wrappers for backward compatibility

## Benefits of This Architecture

### 1. **Testability**
Each handler can be unit tested in isolation:

```csharp
[Test]
public async Task CreateEventCommand_ValidData_CreatesEvent()
{
    // Arrange
    var mockRepo = new Mock<IEventRepository>();
    var handler = new CreateEventCommandHandler(mockRepo.Object, logger);
    var command = new CreateEventCommand(validEventDto);
    
    // Act
    var result = await handler.Handle(command);
    
    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Once);
}
```

### 2. **Maintainability**
- Easy to find specific operations
- Clear naming conventions
- Single responsibility per handler

### 3. **Flexibility**
- Can add caching to specific queries
- Can add authorization to specific commands
- Can implement retry logic per operation

### 4. **Domain-Driven Design**
Commands represent business operations in domain language:
- `CreateEventCommand`
- `UpdateEventCommand`
- `CheckInStaffCommand`

## Migration Path

The implementation maintains **full backward compatibility**:

1. **Old code continues to work**: `ICreateEventUseCase` still functions (being phased out)
2. **New code uses CQRS**: Directly inject and use handlers
3. **Gradual migration**: Update components one at a time

**Note**: `IEventService` has been removed. Use CQRS command and query handlers directly.

## Available Operations

### Events
**Commands:**
- `CreateEventCommand` - Create a new event
- `UpdateEventCommand` - Update an existing event
- `DeleteEventCommand` - Delete an event

**Queries:**
- `GetAllEventsQuery` - Get all events
- `GetEventByIdQuery` - Get event by ID
- `GetUpcomingEventsQuery` - Get upcoming events
- `GetEventsByDateRangeQuery` - Get events in a date range

### Shifts
**Commands:**
- `CreateShiftCommand` - Create a new shift
- `UpdateShiftCommand` - Update a shift
- `DeleteShiftCommand` - Delete a shift

**Queries:**
- `GetAllShiftsQuery` - Get all shifts
- `GetShiftByIdQuery` - Get shift by ID
- `GetShiftsByEventIdQuery` - Get shifts for an event
- `GetUpcomingShiftsQuery` - Get upcoming shifts

### Staff
**Commands:**
- `CreateStaffCommand` - Create a staff member
- `UpdateStaffCommand` - Update staff details
- `DeleteStaffCommand` - Remove a staff member

**Queries:**
- `GetAllStaffQuery` - Get all staff
- `GetStaffByIdQuery` - Get staff by ID
- `GetActiveStaffQuery` - Get active staff members

### Staff Assignments
**Commands:**
- `CreateStaffAssignmentCommand` - Assign staff to shift
- `CheckInStaffCommand` - Check in staff member
- `CheckOutStaffCommand` - Check out staff member

**Queries:**
- `GetAllAssignmentsQuery` - Get all assignments
- `GetAssignmentsByShiftIdQuery` - Get assignments for a shift
- `GetAssignmentsByStaffIdQuery` - Get assignments for staff member

## Best Practices

1. **Commands should be immutable**: Use `record` types
2. **Queries should not modify state**: No side effects
3. **One handler per command/query**: Single responsibility
4. **Validate in handlers**: Business logic stays in the application layer
5. **Use descriptive names**: Commands are verbs, queries describe what data they get
6. **Keep handlers focused**: Each handler does one thing well

## Future Enhancements

The CQRS pattern enables:
- **MediatR integration**: Add a mediator for cross-cutting concerns
- **Event sourcing**: Store commands as events
- **CQRS with separate read/write models**: Optimize each side independently
- **Distributed systems**: Commands and queries can run on different servers
- **Audit logging**: Intercept all commands for compliance
- **Authorization**: Apply permissions per command type

## Summary

The Application layer now uses CQRS to provide:
- ✅ Clear separation between reads and writes
- ✅ Improved testability and maintainability
- ✅ Better alignment with business operations
- ✅ Foundation for future architectural patterns
- ✅ Full backward compatibility with existing code
