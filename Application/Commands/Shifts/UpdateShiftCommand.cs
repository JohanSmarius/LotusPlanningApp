using Application.Common;
using Entities;

namespace Application.Commands.Shifts;

/// <summary>
/// Command to update an existing shift
/// </summary>
public record UpdateShiftCommand(Shift Shift) : ICommand<Shift>;
