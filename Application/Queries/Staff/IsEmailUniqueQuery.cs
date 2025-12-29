using Application.Common;

namespace Application.Queries.Staff;

/// <summary>
/// Query to check if an email address is unique (not already in use by another staff member)
/// </summary>
public record IsEmailUniqueQuery(string Email, int? ExcludeStaffId = null) : IQuery<bool>;
