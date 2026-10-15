namespace LotusPlanningApp.Configuration;

/// <summary>
/// Configuration options for the seeded admin account.
/// </summary>
public class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    /// <summary>
    /// Username and email address for the seeded admin account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for the seeded admin account.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
