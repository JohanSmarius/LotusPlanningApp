using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LotusPlanningApp.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext during migrations.
/// This is used by EF Core tools to create the DbContext at design-time.
/// Supports Aspire connection string parameters.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private static IConfiguration? _configuration;
    
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Build configuration to support Aspire parameters (cached for performance)
        _configuration ??= new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        
        // DESIGN-TIME ONLY: This connection string is only used by EF Core tools for generating migrations.
        // At runtime, the actual connection string is provided by Aspire orchestration.
        
        // Try to get connection string from Aspire configuration first
        var connectionString = _configuration.GetConnectionString("lotusdb");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback to building connection string from environment variables
            var password = Environment.GetEnvironmentVariable("SA_PASSWORD");
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException(
                    "Connection string 'lotusdb' not found and SA_PASSWORD environment variable is not set. " +
                    "Either set the connection string via command line (--ConnectionStrings:lotusdb \"...\") " +
                    "or set the SA_PASSWORD environment variable. " +
                    "Example: export SA_PASSWORD=\"YourSecurePassword\"");
            }
            
            connectionString = $"Server=localhost;Database=lotusdb;User Id=sa;Password={password};TrustServerCertificate=True";
        }
        
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
