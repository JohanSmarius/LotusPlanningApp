using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LotusPlanningApp.Data;

/// <summary>
/// Creates <see cref="ApplicationDbContext"/> instances for EF Core design-time tooling.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <inheritdoc />
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var startupProjectPath = ResolveStartupProjectPath();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(startupProjectPath, "appsettings.json"), optional: false)
            .AddJsonFile(Path.Combine(startupProjectPath, "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveStartupProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidatePaths = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, ".."),
            Path.Combine(currentDirectory, "..", ".."),
            Path.Combine(currentDirectory, "..", "..", ".."),
            Path.Combine(currentDirectory, "..", "LotusPlanningApp", "LotusPlanningApp"),
            Path.Combine(currentDirectory, "..", "..", "LotusPlanningApp", "LotusPlanningApp"),
            Path.Combine(currentDirectory, "..", "..", "..", "LotusPlanningApp", "LotusPlanningApp")
        };

        foreach (var candidatePath in candidatePaths.Select(Path.GetFullPath).Distinct())
        {
            if (File.Exists(Path.Combine(candidatePath, "LotusPlanningApp.csproj")) &&
                File.Exists(Path.Combine(candidatePath, "appsettings.json")))
            {
                return candidatePath;
            }
        }

        throw new InvalidOperationException("Could not locate the LotusPlanningApp startup project for EF Core design-time services.");
    }
}
