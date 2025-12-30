namespace Application.DataAdapters;

/// <summary>
/// Data transfer object for staff hours per year
/// </summary>
public class StaffHoursDTO
{
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Year { get; set; }
    public double TotalHours { get; set; }
    public int TotalShifts { get; set; }
}
