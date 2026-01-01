using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for deleting a customer
/// </summary>
public class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        ICustomerRepository repository,
        ILogger<DeleteCustomerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetCustomerByIdAsync(command.CustomerId);
        if (customer == null)
        {
            throw new ApplicationLayerException($"Customer with ID {command.CustomerId} not found.");
        }

        // Check if customer has events
        if (customer.Events.Any())
        {
            throw new ApplicationLayerException($"Cannot delete customer {customer.FullName} because they have {customer.Events.Count} event(s) associated with them.");
        }

        var result = await _repository.DeleteCustomerAsync(command.CustomerId);

        if (result)
        {
            _logger.LogInformation("Customer deleted: {CustomerId} - {CustomerName}", command.CustomerId, customer.FullName);
        }

        return result;
    }
}
