using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Handler for getting active staff members
/// </summary>
public class GetActiveStaffQueryHandler : IQueryHandler<GetActiveStaffQuery, List<Entities.Staff>>
{
    private readonly IStaffRepository _repository;

    public GetActiveStaffQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Entities.Staff>> Handle(GetActiveStaffQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetActiveStaffAsync();
    }
}
