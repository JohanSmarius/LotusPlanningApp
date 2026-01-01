using Application.Common;

namespace Application.Commands.Customers;

/// <summary>
/// Command to delete a customer
/// </summary>
public record DeleteCustomerCommand(int CustomerId) : ICommand<bool>;
