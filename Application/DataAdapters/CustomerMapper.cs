using Entities;

namespace Application.DataAdapters;

/// <summary>
/// Mapper for converting between Customer entity and CustomerDTO
/// </summary>
public static class CustomerMapper
{
    /// <summary>
    /// Convert Customer entity to CustomerDTO
    /// </summary>
    public static CustomerDTO ToDTO(Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Company = customer.Company,
            Address = customer.Address,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            UserId = customer.UserId
        };
    }

    /// <summary>
    /// Convert CustomerDTO to Customer entity
    /// </summary>
    public static Customer ToEntity(CustomerDTO dto)
    {
        return new Customer
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Company = dto.Company,
            Address = dto.Address,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            UserId = dto.UserId
        };
    }

    /// <summary>
    /// Update existing customer entity from DTO
    /// </summary>
    public static void UpdateFromDTO(Customer customer, CustomerDTO dto)
    {
        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.Company = dto.Company;
        customer.Address = dto.Address;
        customer.City = dto.City;
        customer.PostalCode = dto.PostalCode;
        customer.Country = dto.Country;
        customer.UserId = dto.UserId;
    }
}
