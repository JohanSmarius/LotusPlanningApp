using Application.Common;
using Application.DataAdapters;
using Application.Queries.Customers;

namespace Application.Queries.Customers;

/// <summary>
/// Handler for getting customer by ID
/// </summary>
public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDTO?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByIdQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDTO?> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetCustomerByIdAsync(query.CustomerId);
        return customer != null ? CustomerMapper.ToDTO(customer) : null;
    }
}
