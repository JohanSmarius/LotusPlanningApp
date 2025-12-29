using Application.Common;

namespace Application;

/// <summary>
/// Simple command dispatcher for dispatching commands to their handlers
/// Used in Blazor components to invoke CQRS commands
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command and returns the result
    /// </summary>
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}

/// <summary>
/// Command dispatcher implementation using service locator pattern
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler found for command {typeof(TCommand).Name}");

        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} does not have a Handle method");

        var result = handleMethod.Invoke(handler, new object[] { command, cancellationToken });
        if (result is Task<TResult> task)
            return await task;

        throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} did not return a Task<{typeof(TResult).Name}>");
    }
}
