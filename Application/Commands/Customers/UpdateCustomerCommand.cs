using Application.Common;
using Application.DataAdapters;

namespace Application.Commands.Customers;

/// <summary>
/// Command to update an existing customer
/// </summary>
public record UpdateCustomerCommand(CustomerDTO CustomerData) : ICommand<CustomerDTO>;
