using Application.Common;
using Entities;

namespace Application.Commands.Staff;

/// <summary>
/// Command to create a new staff member
/// </summary>
public record CreateStaffCommand(Entities.Staff Staff) : ICommand<Entities.Staff>;
