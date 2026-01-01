using Application.Commands.Customers;
using Application;
using Entities;
using Infrastructure.Commands.Customers;
using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Commands.Customers;

public class LinkUserToCustomerByEmailCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<LinkUserToCustomerByEmailCommandHandler>> _mockLogger;
    private readonly LinkUserToCustomerByEmailCommandHandler _handler;

    public LinkUserToCustomerByEmailCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<LinkUserToCustomerByEmailCommandHandler>>();
        _handler = new LinkUserToCustomerByEmailCommandHandler(
            _mockUserManager.Object, _mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_EmptyEmail_ReturnsFalse()
    {
        // Arrange
        var command = new LinkUserToCustomerByEmailCommand("user123", "");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var command = new LinkUserToCustomerByEmailCommand("user123", "test@example.com");
        _mockUserManager.Setup(m => m.FindByIdAsync("user123")).ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_CreatesNewCustomer()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var command = new LinkUserToCustomerByEmailCommand("user123", "test@example.com");
        
        _mockUserManager.Setup(m => m.FindByIdAsync("user123")).ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetCustomerByEmailAsync("test@example.com")).ReturnsAsync((Customer?)null);
        _mockRepository.Setup(r => r.CreateCustomerAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => { c.Id = 1; return c; });
        _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.CreateCustomerAsync(It.Is<Customer>(c => 
            c.FirstName == "John" && 
            c.LastName == "Doe" && 
            c.Email == "test@example.com")), Times.Once);
        _mockUserManager.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u => u.CustomerId == 1)), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerAlreadyLinkedToDifferentUser_ReturnsFalse()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user123", Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            UserId = "differentUser"
        };
        var command = new LinkUserToCustomerByEmailCommand("user123", "test@example.com");
        
        _mockUserManager.Setup(m => m.FindByIdAsync("user123")).ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetCustomerByEmailAsync("test@example.com")).ReturnsAsync(customer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CustomerExists_LinksToUser()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user123", Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            UserId = null
        };
        var command = new LinkUserToCustomerByEmailCommand("user123", "test@example.com");
        
        _mockUserManager.Setup(m => m.FindByIdAsync("user123")).ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetCustomerByEmailAsync("test@example.com")).ReturnsAsync(customer);
        _mockRepository.Setup(r => r.UpdateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(customer);
        _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateCustomerAsync(It.Is<Customer>(c => c.UserId == "user123")), Times.Once);
        _mockUserManager.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u => u.CustomerId == 1)), Times.Once);
    }
}
