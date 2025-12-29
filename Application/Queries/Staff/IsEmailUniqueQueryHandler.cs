using Application.Common;

namespace Application.Queries.Staff;

/// <summary>
/// Handler for checking email uniqueness
/// </summary>
public class IsEmailUniqueQueryHandler : IQueryHandler<IsEmailUniqueQuery, bool>
{
    private readonly IStaffRepository _staffRepository;

    public IsEmailUniqueQueryHandler(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    public async Task<bool> Handle(IsEmailUniqueQuery query, CancellationToken cancellationToken = default)
    {
        return await _staffRepository.IsEmailUniqueAsync(query.Email, query.ExcludeStaffId);
    }
}
