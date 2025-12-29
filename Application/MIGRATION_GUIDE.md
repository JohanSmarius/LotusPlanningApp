# Migration Guide: Moving to CQRS

This guide helps you migrate existing code to use the new CQRS pattern.

## Quick Reference

### Before (Legacy)
```csharp
@inject IEventRepository EventRepository

private async Task LoadEvents()
{
    events = await EventRepository.GetAllEventsAsync();
}
```

### After (CQRS)
```csharp
@inject GetAllEventsQueryHandler GetAllEventsHandler

private async Task LoadEvents()
{
    var query = new GetAllEventsQuery();
    events = await GetAllEventsHandler.Handle(query);
}
```

## Step-by-Step Migration

### 1. Events Page Example

**Old Code:**
```csharp
@page "/events"
@inject IEventRepository EventService
@inject ICreateEventUseCase CreateEventUseCase

private List<Event> events = new();

protected override async Task OnInitializedAsync()
{
    events = await EventService.GetAllEventsAsync();
}

private async Task CreateEvent()
{
    var result = await CreateEventUseCase.Execute(newEventDto);
}
```

**New Code (Recommended):**
```csharp
@page "/events"
@inject GetAllEventsQueryHandler GetAllEventsHandler
@inject CreateEventCommandHandler CreateEventHandler

private List<Event> events = new();

protected override async Task OnInitializedAsync()
{
    var query = new GetAllEventsQuery();
    events = await GetAllEventsHandler.Handle(query);
}

private async Task CreateEvent()
{
    var command = new CreateEventCommand(newEventDto);
    var result = await CreateEventHandler.Handle(command);
}
```

### 2. Update Operations

**Old:**
```csharp
@inject IUpdateEventUseCase UpdateEventUseCase

private async Task UpdateEvent()
{
    await UpdateEventUseCase.Execute(updatedEvent);
}
```

**New:**
```csharp
@inject UpdateEventCommandHandler UpdateEventHandler

private async Task UpdateEvent()
{
    var command = new UpdateEventCommand(updatedEvent);
    await UpdateEventHandler.Handle(command);
}
```

### 3. Delete Operations

**Old:**
```csharp
@inject IEventRepository EventRepository

private async Task DeleteEvent(int id)
{
    await EventRepository.DeleteEventAsync(id);
}
```

**New:**
```csharp
@inject DeleteEventCommandHandler DeleteEventHandler

private async Task DeleteEvent(int id)
{
    var command = new DeleteEventCommand(id);
    await DeleteEventHandler.Handle(command);
}
```

## Complete Component Examples

### Events List Component

```csharp
@page "/events"
@using Application.Queries.Events
@using Application.Commands.Events
@using Entities

@inject GetAllEventsQueryHandler GetAllEventsHandler
@inject GetUpcomingEventsQueryHandler GetUpcomingEventsHandler
@inject CreateEventCommandHandler CreateEventHandler
@inject DeleteEventCommandHandler DeleteEventHandler

<h3>Events</h3>

@if (loading)
{
    <p>Loading...</p>
}
else
{
    <button @onclick="LoadUpcomingOnly">Show Upcoming</button>
    <button @onclick="LoadAllEvents">Show All</button>
    
    @foreach (var evt in events)
    {
        <div>
            <h4>@evt.Name</h4>
            <button @onclick="() => DeleteEvent(evt.Id)">Delete</button>
        </div>
    }
}

@code {
    private List<Event> events = new();
    private bool loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadAllEvents();
    }

    private async Task LoadAllEvents()
    {
        loading = true;
        var query = new GetAllEventsQuery();
        events = await GetAllEventsHandler.Handle(query);
        loading = false;
    }

    private async Task LoadUpcomingOnly()
    {
        loading = true;
        var query = new GetUpcomingEventsQuery();
        events = await GetUpcomingEventsHandler.Handle(query);
        loading = false;
    }

    private async Task DeleteEvent(int id)
    {
        var command = new DeleteEventCommand(id);
        var success = await DeleteEventHandler.Handle(command);
        if (success)
        {
            await LoadAllEvents();
        }
    }
}
```

### Staff Management Component

```csharp
@page "/staff"
@using Application.Queries.Staff
@using Application.Commands.Staff
@using Entities

@inject GetAllStaffQueryHandler GetAllStaffHandler
@inject GetActiveStaffQueryHandler GetActiveStaffHandler
@inject CreateStaffCommandHandler CreateStaffHandler
@inject UpdateStaffCommandHandler UpdateStaffHandler

<h3>Staff Management</h3>

<button @onclick="CreateNewStaff">Add Staff</button>

@foreach (var staff in staffList)
{
    <div>
        <p>@staff.FirstName @staff.LastName - @staff.Email</p>
        <button @onclick="() => EditStaff(staff)">Edit</button>
    </div>
}

@code {
    private List<Entities.Staff> staffList = new();

    protected override async Task OnInitializedAsync()
    {
        var query = new GetActiveStaffQuery();
        staffList = await GetActiveStaffHandler.Handle(query);
    }

    private async Task CreateNewStaff()
    {
        var newStaff = new Entities.Staff
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Role = StaffRole.FirstAider
        };

        var command = new CreateStaffCommand(newStaff);
        var created = await CreateStaffHandler.Handle(command);
    }

    private async Task EditStaff(Entities.Staff staff)
    {
        staff.UpdatedAt = DateTime.UtcNow;
        
        var command = new UpdateStaffCommand(staff);
        var updated = await UpdateStaffHandler.Handle(command);
    }
}
```

### Shift Details Component

```csharp
@page "/shifts/{ShiftId:int}"
@using Application.Queries.Shifts
@using Application.Queries.StaffAssignments
@using Application.Commands.StaffAssignments

@inject GetShiftByIdQueryHandler GetShiftByIdHandler
@inject GetAssignmentsByShiftIdQueryHandler GetAssignmentsByShiftIdHandler
@inject CheckInStaffCommandHandler CheckInStaffHandler
@inject CheckOutStaffCommandHandler CheckOutStaffHandler

@if (shift != null)
{
    <h3>@shift.Name</h3>
    <p>@shift.StartTime - @shift.EndTime</p>

    <h4>Assigned Staff</h4>
    @foreach (var assignment in assignments)
    {
        <div>
            <p>Staff: @assignment.Staff.FirstName @assignment.Staff.LastName</p>
            <p>Status: @assignment.Status</p>
            
            @if (assignment.Status == AssignmentStatus.Assigned)
            {
                <button @onclick="() => CheckIn(assignment.Id)">Check In</button>
            }
            else if (assignment.Status == AssignmentStatus.CheckedIn)
            {
                <button @onclick="() => CheckOut(assignment.Id)">Check Out</button>
            }
        </div>
    }
}

@code {
    [Parameter]
    public int ShiftId { get; set; }

    private Shift? shift;
    private List<StaffAssignment> assignments = new();

    protected override async Task OnInitializedAsync()
    {
        var shiftQuery = new GetShiftByIdQuery(ShiftId);
        shift = await GetShiftByIdHandler.Handle(shiftQuery);

        var assignmentsQuery = new GetAssignmentsByShiftIdQuery(ShiftId);
        assignments = await GetAssignmentsByShiftIdHandler.Handle(assignmentsQuery);
    }

    private async Task CheckIn(int assignmentId)
    {
        var command = new CheckInStaffCommand(assignmentId);
        var updated = await CheckInStaffHandler.Handle(command);
        
        if (updated != null)
        {
            // Reload assignments
            var query = new GetAssignmentsByShiftIdQuery(ShiftId);
            assignments = await GetAssignmentsByShiftIdHandler.Handle(query);
        }
    }

    private async Task CheckOut(int assignmentId)
    {
        var command = new CheckOutStaffCommand(assignmentId);
        var updated = await CheckOutStaffHandler.Handle(command);
        
        if (updated != null)
        {
            var query = new GetAssignmentsByShiftIdQuery(ShiftId);
            assignments = await GetAssignmentsByShiftIdHandler.Handle(query);
        }
    }
}
```

## Import Statements

Add these using directives to your components:

```csharp
// For commands
@using Application.Commands.Events
@using Application.Commands.Shifts
@using Application.Commands.Staff
@using Application.Commands.StaffAssignments

// For queries
@using Application.Queries.Events
@using Application.Queries.Shifts
@using Application.Queries.Staff
@using Application.Queries.StaffAssignments

// For entities
@using Entities
```

## Dependency Injection Setup

Already configured in `Program.cs`:

```csharp
// This single line registers all handlers
builder.Services.AddApplicationLayer();
```

## Common Patterns

### Pattern 1: Load Data on Initialize
```csharp
protected override async Task OnInitializedAsync()
{
    var query = new GetAllEventsQuery();
    items = await QueryHandler.Handle(query);
}
```

### Pattern 2: Create with Validation
```csharp
private async Task Create()
{
    if (!ValidateInput())
        return;
        
    try
    {
        var command = new CreateCommand(data);
        var result = await CommandHandler.Handle(command);
        // Success logic
    }
    catch (ApplicationLayerException ex)
    {
        errorMessage = ex.Message;
    }
}
```

### Pattern 3: Update and Reload
```csharp
private async Task Update()
{
    var command = new UpdateCommand(item);
    await CommandHandler.Handle(command);
    
    // Reload to see changes
    await LoadData();
}
```

### Pattern 4: Delete with Confirmation
```csharp
private async Task Delete(int id)
{
    if (await ConfirmDelete())
    {
        var command = new DeleteCommand(id);
        var success = await CommandHandler.Handle(command);
        
        if (success)
        {
            items.RemoveAll(i => i.Id == id);
        }
    }
}
```

## Benefits You Get

1. **Type Safety**: Commands and queries are strongly typed
2. **IntelliSense**: Better IDE support and discoverability
3. **Testability**: Easy to mock handlers in tests
4. **Clear Intent**: Code explicitly shows what operation is being performed
5. **Validation**: All validation logic centralized in handlers
6. **Logging**: Handlers log operations automatically

## Troubleshooting

### "Handler not found"
Make sure you've registered services:
```csharp
builder.Services.AddApplicationLayer();
```

### "Namespace conflict with Staff"
Use fully qualified names:
```csharp
@using Entities.Staff
// Or in code:
private Entities.Staff staff;
```

### "Can't inject handler"
Make sure you have the correct using directive:
```csharp
@using Application.Commands.Events
@inject CreateEventCommandHandler Handler
```

## Migration Checklist

- [ ] Add using directives for Commands and Queries
- [ ] Replace repository injections with handler injections
- [ ] Update method calls to use Command/Query pattern
- [ ] Test all operations still work
- [ ] Remove unused old service injections
- [ ] Update error handling if needed

## Need Help?

- See [README_CQRS.md](README_CQRS.md) for architecture details
- Check handler implementations in `Commands/` and `Queries/` folders
- Review existing command/query patterns for guidance
