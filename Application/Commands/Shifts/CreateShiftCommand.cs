using Application.Common;
using Entities;

namespace Application.Commands.Shifts;

/// <summary>
/// Command to create a new shift
/// </summary>
public record CreateShiftCommand(Shift Shift) : ICommand<Shift>;
