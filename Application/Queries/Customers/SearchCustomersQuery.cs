using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.Customers;

/// <summary>
/// Query to search customers by name or email
/// </summary>
public record SearchCustomersQuery(string SearchTerm) : IQuery<List<CustomerDTO>>;
