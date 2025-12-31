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
using Application.Common;
using Application.DataAdapters;
using Entities;

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
        services.AddScoped(typeof(ICommandHandler<CreateEventCommand, EventDTO>), typeof(CreateEventCommandHandler));
        services.AddScoped(typeof(ICommandHandler<UpdateEventCommand, Event>), typeof(UpdateEventCommandHandler));
        services.AddScoped(typeof(ICommandHandler<DeleteEventCommand, bool>), typeof(DeleteEventCommandHandler));

        // Register Event Query Handlers
        services.AddScoped(typeof(IQueryHandler<GetAllEventsQuery, List<Event>>), typeof(GetAllEventsQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetEventByIdQuery, Event?>), typeof(GetEventByIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetUpcomingEventsQuery, List<Event>>), typeof(GetUpcomingEventsQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetEventsByDateRangeQuery, List<Event>>), typeof(GetEventsByDateRangeQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetEventsByCustomerIdQuery, List<EventDTO>>), typeof(GetEventsByCustomerIdQueryHandler));

        // Register Shift Command Handlers
        services.AddScoped(typeof(ICommandHandler<CreateShiftCommand, Shift>), typeof(CreateShiftCommandHandler));
        services.AddScoped(typeof(ICommandHandler<UpdateShiftCommand, Shift>), typeof(UpdateShiftCommandHandler));
        services.AddScoped(typeof(ICommandHandler<DeleteShiftCommand, bool>), typeof(DeleteShiftCommandHandler));

        // Register Shift Query Handlers
        services.AddScoped(typeof(IQueryHandler<GetAllShiftsQuery, List<Shift>>), typeof(GetAllShiftsQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetShiftByIdQuery, Shift?>), typeof(GetShiftByIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetShiftsByEventIdQuery, List<Shift>>), typeof(GetShiftsByEventIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetUpcomingShiftsQuery, List<Shift>>), typeof(GetUpcomingShiftsQueryHandler));

        // Register Staff Command Handlers
        services.AddScoped(typeof(ICommandHandler<CreateStaffCommand, Entities.Staff>), typeof(CreateStaffCommandHandler));
        services.AddScoped(typeof(ICommandHandler<UpdateStaffCommand, Entities.Staff>), typeof(UpdateStaffCommandHandler));
        services.AddScoped(typeof(ICommandHandler<DeleteStaffCommand, bool>), typeof(DeleteStaffCommandHandler));

        // Register Staff Query Handlers
        services.AddScoped(typeof(IQueryHandler<GetAllStaffQuery, List<Entities.Staff>>), typeof(GetAllStaffQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetStaffByIdQuery, Entities.Staff?>), typeof(GetStaffByIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetActiveStaffQuery, List<Entities.Staff>>), typeof(GetActiveStaffQueryHandler));
        services.AddScoped(typeof(IQueryHandler<IsEmailUniqueQuery, bool>), typeof(IsEmailUniqueQueryHandler));

        // Register Staff Assignment Command Handlers
        services.AddScoped(typeof(ICommandHandler<CreateStaffAssignmentCommand, StaffAssignment>), typeof(CreateStaffAssignmentCommandHandler));
        services.AddScoped(typeof(ICommandHandler<CheckInStaffCommand, StaffAssignment?>), typeof(CheckInStaffCommandHandler));
        services.AddScoped(typeof(ICommandHandler<CheckOutStaffCommand, StaffAssignment?>), typeof(CheckOutStaffCommandHandler));
        services.AddScoped(typeof(ICommandHandler<DeleteStaffAssignmentCommand, bool>), typeof(DeleteStaffAssignmentCommandHandler));

        // Register Staff Assignment Query Handlers
        services.AddScoped(typeof(IQueryHandler<GetAllAssignmentsQuery, List<StaffAssignment>>), typeof(GetAllAssignmentsQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetAssignmentsByShiftIdQuery, List<StaffAssignment>>), typeof(GetAssignmentsByShiftIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetAssignmentsByStaffIdQuery, List<StaffAssignment>>), typeof(GetAssignmentsByStaffIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetConfirmedAssignmentsByStaffIdQuery, List<StaffAssignment>>), typeof(GetConfirmedAssignmentsByStaffIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<IsStaffAvailableQuery, bool>), typeof(IsStaffAvailableQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetStaffHoursPerYearQuery, List<StaffHoursDTO>>), typeof(GetStaffHoursPerYearQueryHandler));

        // Register Calendar Query Handlers
        services.AddScoped(typeof(IQueryHandler<GenerateShiftIcsQuery, string?>), typeof(GenerateShiftIcsQueryHandler));

        // Register Customer Command Handlers
        services.AddScoped(typeof(ICommandHandler<CreateCustomerCommand, CustomerDTO>), typeof(CreateCustomerCommandHandler));
        services.AddScoped(typeof(ICommandHandler<LinkCustomerToEventCommand, bool>), typeof(LinkCustomerToEventCommandHandler));
        services.AddScoped(typeof(ICommandHandler<RequestEventCancellationCommand, bool>), typeof(RequestEventCancellationCommandHandler));

        // Register Customer Query Handlers
        services.AddScoped(typeof(IQueryHandler<GetAllCustomersQuery, List<CustomerDTO>>), typeof(GetAllCustomersQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetCustomerByIdQuery, CustomerDTO?>), typeof(GetCustomerByIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<GetCustomerByUserIdQuery, CustomerDTO?>), typeof(GetCustomerByUserIdQueryHandler));
        services.AddScoped(typeof(IQueryHandler<SearchCustomersQuery, List<CustomerDTO>>), typeof(SearchCustomersQueryHandler));

        // Register legacy wrappers for backward compatibility
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ICreateEventUseCase, CreateEventUseCase>();
        services.AddScoped<IUpdateEventUseCase, UpdateEventUseCase>();

        return services;
    }
}
