using Application.Commands.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Application.Tests;

public class EventDomainServiceIntegrationTests
{
    [Fact]
    public void AddApplicationLayer_RegistersEventDomainService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationLayer();
        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        // Assert
        _ = scope.ServiceProvider.GetRequiredService<EventDomainService>();
    }

    [Fact]
    public void UpdateEventCommandHandler_Constructor_IncludesEventDomainServiceDependency()
    {
        // Arrange
        var constructor = typeof(UpdateEventCommandHandler).GetConstructors().Single();

        // Act
        var hasDomainServiceDependency = constructor.GetParameters()
            .Any(p => p.ParameterType == typeof(EventDomainService));

        // Assert
        Assert.True(hasDomainServiceDependency);
    }
}
