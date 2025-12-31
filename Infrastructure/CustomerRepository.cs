using Application;
using Entities;
using LotusPlanningApp.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Repository implementation for customer data access
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.Events)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(string userId)
    {
        return await _context.Customers
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> DeleteCustomerAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.Customers
            .Where(c => c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                       c.LastName.ToLower().Contains(lowerSearchTerm) ||
                       c.Email.ToLower().Contains(lowerSearchTerm) ||
                       (c.Company != null && c.Company.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }
}
