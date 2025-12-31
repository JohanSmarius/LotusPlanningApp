using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Represents a customer who can create events
/// </summary>
public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Reference to the user account associated with this customer
    /// </summary>
    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Full name property
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties
    public List<Event> Events { get; set; } = new();
}
