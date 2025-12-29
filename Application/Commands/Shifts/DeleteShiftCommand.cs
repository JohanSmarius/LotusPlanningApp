using Application.Common;

namespace Application.Commands.Shifts;

/// <summary>
/// Command to delete a shift
/// </summary>
public record DeleteShiftCommand(int ShiftId) : ICommand<bool>;
