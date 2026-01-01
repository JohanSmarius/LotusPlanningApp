using Application.Common;

namespace Application.Commands.Customers;

/// <summary>
/// Command to link a user account to a customer by email
/// </summary>
public record LinkUserToCustomerByEmailCommand(string UserId, string Email) : ICommand<bool>;
