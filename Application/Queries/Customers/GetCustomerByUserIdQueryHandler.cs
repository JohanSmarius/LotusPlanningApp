using Application.Common;
using Application.DataAdapters;
using Application.Queries.Customers;

namespace Application.Queries.Customers;

/// <summary>
/// Handler for getting customer by user ID
/// </summary>
public class GetCustomerByUserIdQueryHandler : IQueryHandler<GetCustomerByUserIdQuery, CustomerDTO?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByUserIdQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDTO?> Handle(GetCustomerByUserIdQuery query, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetCustomerByUserIdAsync(query.UserId);
        return customer != null ? CustomerMapper.ToDTO(customer) : null;
    }
}
