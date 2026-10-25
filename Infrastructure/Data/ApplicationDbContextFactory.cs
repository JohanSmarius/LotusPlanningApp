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
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(startupProjectPath, "appsettings.json"), optional: false);

        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            configurationBuilder.AddJsonFile(
                Path.Combine(startupProjectPath, $"appsettings.{environmentName}.json"),
                optional: true);
        }

        var configuration = configurationBuilder
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
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            var startupProject = directory
                .EnumerateFiles("LotusPlanningApp.csproj", SearchOption.AllDirectories)
                .FirstOrDefault(file => file.Directory is not null &&
                                        File.Exists(Path.Combine(file.Directory.FullName, "appsettings.json")));

            if (startupProject?.Directory is not null)
            {
                return startupProject.Directory.FullName;
            }
        }

        throw new InvalidOperationException("Could not locate the LotusPlanningApp startup project for EF Core design-time services.");
    }
}
