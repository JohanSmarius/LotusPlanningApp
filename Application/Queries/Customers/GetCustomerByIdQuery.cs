using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.Customers;

/// <summary>
/// Query to get customer by ID
/// </summary>
public record GetCustomerByIdQuery(int CustomerId) : IQuery<CustomerDTO?>;
