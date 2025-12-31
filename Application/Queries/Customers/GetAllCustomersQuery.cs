using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.Customers;

/// <summary>
/// Query to get all customers
/// </summary>
public record GetAllCustomersQuery() : IQuery<List<CustomerDTO>>;
