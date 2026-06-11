using Application.Common;
using Moq;
using Xunit;

namespace Application.Tests;

public class ICommandDispatcherTests
{
    // Test command and handler for successful scenarios
    public record TestCommand(string Value) : ICommand<string>;

    public record TestResult(string Value);

    public class TestCommandHandler : ICommandHandler<TestCommand, string>
    {
        private readonly string _result;

        public TestCommandHandler(string result)
        {
            _result = result;
        }

        public Task<string> Handle(TestCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }

    [Fact]
    public void Constructor_WithValidServiceProvider_InitializesSuccessfully()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();

        // Act
        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);

        // Assert
        Assert.NotNull(dispatcher);
    }

    [Fact]
    public async Task DispatchAsync_WithValidHandler_ReturnsResult()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var handler = new TestCommandHandler("TestResult");
        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handler);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new TestCommand("TestValue");

        // Act
        var result = await dispatcher.DispatchAsync<TestCommand, string>(command);

        // Assert
        Assert.Equal("TestResult", result);
    }

    [Fact]
    public async Task DispatchAsync_WithCancellationToken_PassesTokenToHandler()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        var mockHandler = new Mock<ICommandHandler<TestCommand, string>>();
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result")
            .Verifiable();

        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(mockHandler.Object);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new TestCommand("TestValue");

        // Act
        await dispatcher.DispatchAsync<TestCommand, string>(command, cancellationToken);

        // Assert
        mockHandler.Verify(h => h.Handle(command, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns((object?)null);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new TestCommand("TestValue");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync<TestCommand, string>(command));
        
        Assert.Contains("No handler found for command TestCommand", exception.Message);
    }

    [Fact]
    public async Task DispatchAsync_MultipleSequentialCalls_HandlesCorrectly()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var handler = new TestCommandHandler("Result");
        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handler);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);

        // Act
        var result1 = await dispatcher.DispatchAsync<TestCommand, string>(new TestCommand("Value1"));
        var result2 = await dispatcher.DispatchAsync<TestCommand, string>(new TestCommand("Value2"));
        var result3 = await dispatcher.DispatchAsync<TestCommand, string>(new TestCommand("Value3"));

        // Assert
        Assert.Equal("Result", result1);
        Assert.Equal("Result", result2);
        Assert.Equal("Result", result3);
    }

    [Fact]
    public async Task DispatchAsync_WithDifferentResultTypes_HandlesCorrectly()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var handler = new IntCommandHandler(42);
        var handlerType = typeof(ICommandHandler<IntCommand, int>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handler);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new IntCommand(10);

        // Act
        var result = await dispatcher.DispatchAsync<IntCommand, int>(command);

        // Assert
        Assert.Equal(42, result);
    }

    public record IntCommand(int Value) : ICommand<int>;

    public class IntCommandHandler : ICommandHandler<IntCommand, int>
    {
        private readonly int _result;

        public IntCommandHandler(int result)
        {
            _result = result;
        }

        public Task<int> Handle(IntCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }

    [Fact]
    public async Task DispatchAsync_WithComplexResult_ReturnsComplexObject()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var expectedResult = new TestResult("ComplexValue");
        var handler = new ComplexCommandHandler(expectedResult);
        var handlerType = typeof(ICommandHandler<ComplexCommand, TestResult>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handler);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new ComplexCommand("Input");

        // Act
        var result = await dispatcher.DispatchAsync<ComplexCommand, TestResult>(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ComplexValue", result.Value);
    }

    public record ComplexCommand(string Input) : ICommand<TestResult>;

    public class ComplexCommandHandler : ICommandHandler<ComplexCommand, TestResult>
    {
        private readonly TestResult _result;

        public ComplexCommandHandler(TestResult result)
        {
            _result = result;
        }

        public Task<TestResult> Handle(ComplexCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<TestCommand, string>>();
        
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApplicationException("Handler error"));

        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(mockHandler.Object);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new TestCommand("TestValue");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationException>(
            () => dispatcher.DispatchAsync<TestCommand, string>(command));
        
        Assert.Equal("Handler error", exception.Message);
    }

    [Fact]
    public async Task DispatchAsync_WithCancelledToken_PropagatesCancellation()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately
        
        var mockHandler = new Mock<ICommandHandler<TestCommand, string>>();
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var handlerType = typeof(ICommandHandler<TestCommand, string>);
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(mockHandler.Object);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new TestCommand("TestValue");

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => dispatcher.DispatchAsync<TestCommand, string>(command, cts.Token));
    }

    [Fact]
    public async Task DispatchAsync_WithNullableResultType_HandlesCorrectly()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var handler = new NullableCommandHandler(null);
        var handlerType = typeof(ICommandHandler<NullableCommand, string?>);
        
        mockServiceProvider
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handler);

        var dispatcher = new CommandDispatcher(mockServiceProvider.Object);
        var command = new NullableCommand();

        // Act
        var result = await dispatcher.DispatchAsync<NullableCommand, string?>(command);

        // Assert
        Assert.Null(result);
    }

    public record NullableCommand : ICommand<string?>;

    public class NullableCommandHandler : ICommandHandler<NullableCommand, string?>
    {
        private readonly string? _result;

        public NullableCommandHandler(string? result)
        {
            _result = result;
        }

        public Task<string?> Handle(NullableCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }
}
