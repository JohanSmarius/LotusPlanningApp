using Application.Common;
using Entities;

namespace Application.Commands.Staff;

/// <summary>
/// Command to update an existing staff member
/// </summary>
public record UpdateStaffCommand(Entities.Staff Staff) : ICommand<Entities.Staff>;
