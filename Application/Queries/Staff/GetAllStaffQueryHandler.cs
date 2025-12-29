using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Handler for getting all staff members
/// </summary>
public class GetAllStaffQueryHandler : IQueryHandler<GetAllStaffQuery, List<Entities.Staff>>
{
    private readonly IStaffRepository _repository;

    public GetAllStaffQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Entities.Staff>> Handle(GetAllStaffQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllStaffAsync();
    }
}
