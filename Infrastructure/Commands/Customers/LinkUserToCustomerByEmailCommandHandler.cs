using Application;
using Application.Commands.Customers;
using Application.Common;
using Entities;
using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Commands.Customers;

/// <summary>
/// Handles LinkUserToCustomerByEmailCommand
/// Attempts to link a user to an existing customer by email.
/// If no customer is found, creates a new one based on user details.
/// </summary>
public class LinkUserToCustomerByEmailCommandHandler : ICommandHandler<LinkUserToCustomerByEmailCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<LinkUserToCustomerByEmailCommandHandler> _logger;

    public LinkUserToCustomerByEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICustomerRepository customerRepository,
        ILogger<LinkUserToCustomerByEmailCommandHandler> logger)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(LinkUserToCustomerByEmailCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Email))
        {
            _logger.LogWarning("Email is null or empty for user {UserId}", command.UserId);
            return false;
        }

        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return false;
        }

        // Try to find existing customer by email
        var customer = await _customerRepository.GetCustomerByEmailAsync(command.Email);

        // If not found, create a new customer from user data
        if (customer == null)
        {
            customer = new Customer
            {
                FirstName = user.FirstName ?? "Unknown",
                LastName = user.LastName ?? "User",
                Email = command.Email,
                CreatedAt = DateTime.UtcNow
            };

            customer = await _customerRepository.CreateCustomerAsync(customer);
            _logger.LogInformation("Created new customer {CustomerId} for user {UserId} with email {Email}", customer.Id, command.UserId, command.Email);
        }

        // Check if customer is already linked to a different user
        if (customer.UserId != null && customer.UserId != command.UserId)
        {
            _logger.LogWarning("Customer {CustomerId} is already linked to user {ExistingUserId}, cannot link to {NewUserId}", 
                customer.Id, customer.UserId, command.UserId);
            return false;
        }

        // Link user to customer (update if needed)
        if (customer.UserId != command.UserId)
        {
            customer.UserId = command.UserId;
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.UpdateCustomerAsync(customer);
            _logger.LogInformation("Linked user {UserId} to customer {CustomerId}", command.UserId, customer.Id);
        }

        // Link customer to user
        user.CustomerId = customer.Id;
        var result = await _userManager.UpdateAsync(user);
        
        return result.Succeeded;
    }
}
