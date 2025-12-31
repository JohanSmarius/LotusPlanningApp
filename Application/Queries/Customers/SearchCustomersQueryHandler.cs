using Application.Common;
using Application.DataAdapters;
using Application.Queries.Customers;

namespace Application.Queries.Customers;

/// <summary>
/// Handler for searching customers
/// </summary>
public class SearchCustomersQueryHandler : IQueryHandler<SearchCustomersQuery, List<CustomerDTO>>
{
    private readonly ICustomerRepository _repository;

    public SearchCustomersQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDTO>> Handle(SearchCustomersQuery query, CancellationToken cancellationToken = default)
    {
        var customers = await _repository.SearchCustomersAsync(query.SearchTerm);
        return customers.Select(CustomerMapper.ToDTO).ToList();
    }
}
