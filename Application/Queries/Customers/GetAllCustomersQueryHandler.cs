using Application.Common;
using Application.DataAdapters;
using Application.Queries.Customers;

namespace Application.Queries.Customers;

/// <summary>
/// Handler for getting all customers
/// </summary>
public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, List<CustomerDTO>>
{
    private readonly ICustomerRepository _repository;

    public GetAllCustomersQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDTO>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken = default)
    {
        var customers = await _repository.GetAllCustomersAsync();
        return customers.Select(CustomerMapper.ToDTO).ToList();
    }
}
