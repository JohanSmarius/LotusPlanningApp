using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Staff;

/// <summary>
/// Handler for creating a new staff member
/// </summary>
public class CreateStaffCommandHandler : ICommandHandler<CreateStaffCommand, Entities.Staff>
{
    private readonly IStaffRepository _repository;
    private readonly ILogger<CreateStaffCommandHandler> _logger;

    public CreateStaffCommandHandler(
        IStaffRepository repository,
        ILogger<CreateStaffCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Entities.Staff> Handle(CreateStaffCommand command, CancellationToken cancellationToken = default)
    {
        var staff = command.Staff;
        
        // Validate email uniqueness
        if (!await _repository.IsEmailUniqueAsync(staff.Email))
        {
            throw new ApplicationLayerException($"Email {staff.Email} is already in use.");
        }

        staff.CreatedAt = DateTime.UtcNow;
        staff.UpdatedAt = DateTime.UtcNow;

        var createdStaff = await _repository.CreateStaffAsync(staff);
        _logger.LogInformation("Staff member {StaffId} created successfully.", createdStaff.Id);

        return createdStaff;
    }
}
