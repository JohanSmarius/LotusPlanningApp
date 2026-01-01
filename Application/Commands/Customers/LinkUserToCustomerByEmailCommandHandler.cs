using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for linking a user account to a customer by email
/// </summary>
public class LinkUserToCustomerByEmailCommandHandler : ICommandHandler<LinkUserToCustomerByEmailCommand, bool>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<LinkUserToCustomerByEmailCommandHandler> _logger;

    public LinkUserToCustomerByEmailCommandHandler(
        ICustomerRepository repository,
        ILogger<LinkUserToCustomerByEmailCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(LinkUserToCustomerByEmailCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetCustomerByEmailAsync(command.Email);
        if (customer == null)
        {
            _logger.LogWarning("No customer found with email {Email} to link to user {UserId}", command.Email, command.UserId);
            return false;
        }

        // Check if customer is already linked to another user
        if (customer.UserId != null && customer.UserId != command.UserId)
        {
            throw new ApplicationLayerException($"Customer {customer.FullName} is already linked to another user account.");
        }

        // Link user to customer
        customer.UserId = command.UserId;
        customer.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateCustomerAsync(customer);

        _logger.LogInformation("User {UserId} linked to customer {CustomerId} - {CustomerName}", command.UserId, customer.Id, customer.FullName);

        return true;
    }
}
