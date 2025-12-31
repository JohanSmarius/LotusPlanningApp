using Application.Common;

namespace Application.Commands.Customers;

/// <summary>
/// Command to request cancellation of an event
/// </summary>
public record RequestEventCancellationCommand(int EventId, string Reason) : ICommand<bool>;
