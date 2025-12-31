using Application.Common;

namespace Application.Commands.Customers;

/// <summary>
/// Command to link a customer to an event
/// </summary>
public record LinkCustomerToEventCommand(int CustomerId, int EventId) : ICommand<bool>;
