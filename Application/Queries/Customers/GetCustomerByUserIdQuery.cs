using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.Customers;

/// <summary>
/// Query to get customer by user ID
/// </summary>
public record GetCustomerByUserIdQuery(string UserId) : IQuery<CustomerDTO?>;
