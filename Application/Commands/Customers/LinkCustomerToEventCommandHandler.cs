using Application.Commands.Customers;
using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for linking a customer to an event
/// </summary>
public class LinkCustomerToEventCommandHandler : ICommandHandler<LinkCustomerToEventCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<LinkCustomerToEventCommandHandler> _logger;

    public LinkCustomerToEventCommandHandler(
        ICustomerRepository customerRepository,
        IEventRepository eventRepository,
        ILogger<LinkCustomerToEventCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(LinkCustomerToEventCommand command, CancellationToken cancellationToken = default)
    {
        // Validate customer exists
        var customer = await _customerRepository.GetCustomerByIdAsync(command.CustomerId);
        if (customer == null)
        {
            throw new ApplicationLayerException($"Customer with ID {command.CustomerId} not found.");
        }

        // Validate event exists
        var eventEntity = await _eventRepository.GetEventByIdAsync(command.EventId);
        if (eventEntity == null)
        {
            throw new ApplicationLayerException($"Event with ID {command.EventId} not found.");
        }

        // Link customer to event
        eventEntity.CustomerId = command.CustomerId;
        eventEntity.UpdatedAt = DateTime.UtcNow;
        await _eventRepository.UpdateEventAsync(eventEntity);

        _logger.LogInformation("Customer {CustomerId} linked to Event {EventId}", command.CustomerId, command.EventId);

        return true;
    }
}
