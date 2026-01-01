using Application.Common;
using Application.DataAdapters;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for updating an existing customer
/// </summary>
public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, CustomerDTO>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(
        ICustomerRepository repository,
        ILogger<UpdateCustomerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CustomerDTO> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customerData = command.CustomerData;

        // Get existing customer
        var existing = await _repository.GetCustomerByIdAsync(customerData.Id);
        if (existing == null)
        {
            throw new ApplicationLayerException($"Customer with ID {customerData.Id} not found.");
        }

        // Check if email is being changed and if it conflicts with another customer
        if (existing.Email != customerData.Email)
        {
            var emailConflict = await _repository.GetCustomerByEmailAsync(customerData.Email);
            if (emailConflict != null && emailConflict.Id != customerData.Id)
            {
                throw new ApplicationLayerException($"A customer with email {customerData.Email} already exists.");
            }
        }

        // Update entity from DTO
        CustomerMapper.UpdateFromDTO(existing, customerData);
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateCustomerAsync(existing);

        _logger.LogInformation("Customer updated: {CustomerId} - {CustomerName}", updated.Id, updated.FullName);

        return CustomerMapper.ToDTO(updated);
    }
}
