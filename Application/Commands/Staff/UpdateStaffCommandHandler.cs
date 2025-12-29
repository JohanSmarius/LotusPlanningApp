using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Staff;

/// <summary>
/// Handler for updating an existing staff member
/// </summary>
public class UpdateStaffCommandHandler : ICommandHandler<UpdateStaffCommand, Entities.Staff>
{
    private readonly IStaffRepository _repository;
    private readonly ILogger<UpdateStaffCommandHandler> _logger;

    public UpdateStaffCommandHandler(
        IStaffRepository repository,
        ILogger<UpdateStaffCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Entities.Staff> Handle(UpdateStaffCommand command, CancellationToken cancellationToken = default)
    {
        var staff = command.Staff;
        
        // Validate email uniqueness (excluding current staff member)
        if (!await _repository.IsEmailUniqueAsync(staff.Email, staff.Id))
        {
            throw new ApplicationLayerException($"Email {staff.Email} is already in use by another staff member.");
        }

        staff.UpdatedAt = DateTime.UtcNow;

        var updatedStaff = await _repository.UpdateStaffAsync(staff);
        _logger.LogInformation("Staff member {StaffId} updated successfully.", updatedStaff.Id);

        return updatedStaff;
    }
}
