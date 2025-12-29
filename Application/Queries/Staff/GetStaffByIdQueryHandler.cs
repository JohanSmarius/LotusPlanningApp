using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Handler for getting a staff member by ID
/// </summary>
public class GetStaffByIdQueryHandler : IQueryHandler<GetStaffByIdQuery, Entities.Staff?>
{
    private readonly IStaffRepository _repository;

    public GetStaffByIdQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Entities.Staff?> Handle(GetStaffByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetStaffByIdAsync(query.StaffId);
    }
}
