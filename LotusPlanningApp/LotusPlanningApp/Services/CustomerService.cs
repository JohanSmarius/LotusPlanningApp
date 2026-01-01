using Application.DataAdapters;
using Application.Queries.Customers;

namespace LotusPlanningApp.Services;

/// <summary>
/// Service for loading customer data in Blazor components
/// </summary>
public interface ICustomerService
{
    Task<List<CustomerDTO>> GetAllCustomersAsync();
}

public class CustomerService : ICustomerService
{
    private readonly GetAllCustomersQueryHandler _getAllCustomersHandler;

    public CustomerService(GetAllCustomersQueryHandler getAllCustomersHandler)
    {
        _getAllCustomersHandler = getAllCustomersHandler;
    }

    public async Task<List<CustomerDTO>> GetAllCustomersAsync()
    {
        try
        {
            var query = new GetAllCustomersQuery();
            return await _getAllCustomersHandler.Handle(query);
        }
        catch (Exception)
        {
            // Return empty list on error to avoid breaking the UI
            return new List<CustomerDTO>();
        }
    }
}
