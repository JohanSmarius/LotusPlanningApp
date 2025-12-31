using Application.Commands.Customers;
using Application.Common;
using Application.DataAdapters;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for creating a new customer
/// </summary>
public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDTO>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository repository,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CustomerDTO> Handle(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customerData = command.CustomerData;

        // Check if customer with this email already exists
        var existing = await _repository.GetCustomerByEmailAsync(customerData.Email);
        if (existing != null)
        {
            throw new ApplicationLayerException($"A customer with email {customerData.Email} already exists.");
        }

        var entity = CustomerMapper.ToEntity(customerData);
        entity.CreatedAt = DateTime.UtcNow;

        var created = await _repository.CreateCustomerAsync(entity);

        _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", created.Id, created.FullName);

        return CustomerMapper.ToDTO(created);
    }
}
