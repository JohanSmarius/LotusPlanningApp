using Microsoft.Extensions.DependencyInjection;
using Application.Commands.Events;
using Application.Commands.Shifts;
using Application.Commands.Staff;
using Application.Commands.StaffAssignments;
using Application.Commands.Customers;
using Application.Queries.Events;
using Application.Queries.Shifts;
using Application.Queries.Staff;
using Application.Queries.StaffAssignments;
using Application.Queries.Calendar;
using Application.Queries.Customers;

namespace Application;

/// <summary>
/// Extension methods for registering CQRS handlers
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all command and query handlers for the application layer
    /// </summary>
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Register Event Command Handlers
        services.AddScoped<CreateEventCommandHandler>();
        services.AddScoped<UpdateEventCommandHandler>();
        services.AddScoped<DeleteEventCommandHandler>();

        // Register Event Query Handlers
        services.AddScoped<GetAllEventsQueryHandler>();
        services.AddScoped<GetEventByIdQueryHandler>();
        services.AddScoped<GetUpcomingEventsQueryHandler>();
        services.AddScoped<GetEventsByDateRangeQueryHandler>();
        services.AddScoped<GetEventsByCustomerIdQueryHandler>();

        // Register Shift Command Handlers
        services.AddScoped<CreateShiftCommandHandler>();
        services.AddScoped<UpdateShiftCommandHandler>();
        services.AddScoped<DeleteShiftCommandHandler>();

        // Register Shift Query Handlers
        services.AddScoped<GetAllShiftsQueryHandler>();
        services.AddScoped<GetShiftByIdQueryHandler>();
        services.AddScoped<GetShiftsByEventIdQueryHandler>();
        services.AddScoped<GetUpcomingShiftsQueryHandler>();

        // Register Staff Command Handlers
        services.AddScoped<CreateStaffCommandHandler>();
        services.AddScoped<UpdateStaffCommandHandler>();
        services.AddScoped<DeleteStaffCommandHandler>();

        // Register Staff Query Handlers
        services.AddScoped<GetAllStaffQueryHandler>();
        services.AddScoped<GetStaffByIdQueryHandler>();
        services.AddScoped<GetActiveStaffQueryHandler>();
        services.AddScoped<IsEmailUniqueQueryHandler>();

        // Register Staff Assignment Command Handlers
        services.AddScoped<CreateStaffAssignmentCommandHandler>();
        services.AddScoped<CheckInStaffCommandHandler>();
        services.AddScoped<CheckOutStaffCommandHandler>();
        services.AddScoped<DeleteStaffAssignmentCommandHandler>();

        // Register Staff Assignment Query Handlers
        services.AddScoped<GetAllAssignmentsQueryHandler>();
        services.AddScoped<GetAssignmentsByShiftIdQueryHandler>();
        services.AddScoped<GetAssignmentsByStaffIdQueryHandler>();
        services.AddScoped<GetConfirmedAssignmentsByStaffIdQueryHandler>();
        services.AddScoped<IsStaffAvailableQueryHandler>();
        services.AddScoped<GetStaffHoursPerYearQueryHandler>();

        // Register Calendar Query Handlers
        services.AddScoped<GenerateShiftIcsQueryHandler>();

        // Register Customer Command Handlers
        services.AddScoped<CreateCustomerCommandHandler>();
        services.AddScoped<LinkCustomerToEventCommandHandler>();
        services.AddScoped<RequestEventCancellationCommandHandler>();

        // Register Customer Query Handlers
        services.AddScoped<GetAllCustomersQueryHandler>();
        services.AddScoped<GetCustomerByIdQueryHandler>();
        services.AddScoped<GetCustomerByUserIdQueryHandler>();
        services.AddScoped<SearchCustomersQueryHandler>();

        // Register legacy wrappers for backward compatibility
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ICreateEventUseCase, CreateEventUseCase>();
        services.AddScoped<IUpdateEventUseCase, UpdateEventUseCase>();

        return services;
    }
}
