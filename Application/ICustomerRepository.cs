using Entities;

namespace Application;

/// <summary>
/// Repository interface for customer data access
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Get all customers
    /// </summary>
    Task<List<Customer>> GetAllCustomersAsync();

    /// <summary>
    /// Get customer by ID
    /// </summary>
    Task<Customer?> GetCustomerByIdAsync(int customerId);

    /// <summary>
    /// Get customer by user ID
    /// </summary>
    Task<Customer?> GetCustomerByUserIdAsync(string userId);

    /// <summary>
    /// Get customer by email
    /// </summary>
    Task<Customer?> GetCustomerByEmailAsync(string email);

    /// <summary>
    /// Create a new customer
    /// </summary>
    Task<Customer> CreateCustomerAsync(Customer customer);

    /// <summary>
    /// Update an existing customer
    /// </summary>
    Task<Customer> UpdateCustomerAsync(Customer customer);

    /// <summary>
    /// Delete a customer
    /// </summary>
    Task<bool> DeleteCustomerAsync(int customerId);

    /// <summary>
    /// Search customers by name or email
    /// </summary>
    Task<List<Customer>> SearchCustomersAsync(string searchTerm);
}
