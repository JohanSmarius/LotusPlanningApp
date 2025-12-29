using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Staff;

/// <summary>
/// Handler for deleting a staff member
/// </summary>
public class DeleteStaffCommandHandler : ICommandHandler<DeleteStaffCommand, bool>
{
    private readonly IStaffRepository _repository;
    private readonly ILogger<DeleteStaffCommandHandler> _logger;

    public DeleteStaffCommandHandler(
        IStaffRepository repository,
        ILogger<DeleteStaffCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteStaffCommand command, CancellationToken cancellationToken = default)
    {
        var staffId = command.StaffId;
        
        var existingStaff = await _repository.GetStaffByIdAsync(staffId);
        if (existingStaff == null)
        {
            _logger.LogWarning("Attempted to delete non-existent staff member {StaffId}", staffId);
            return false;
        }

        await _repository.DeleteStaffAsync(staffId);
        _logger.LogInformation("Staff member {StaffId} deleted successfully.", staffId);
        
        return true;
    }
}
