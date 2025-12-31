using Application.Common;
using Application.DataAdapters;

namespace Application.Commands.Customers;

/// <summary>
/// Command to create a new customer
/// </summary>
public record CreateCustomerCommand(CustomerDTO CustomerData) : ICommand<CustomerDTO>;
