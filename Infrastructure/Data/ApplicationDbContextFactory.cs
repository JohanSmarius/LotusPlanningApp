using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LotusPlanningApp.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext during migrations.
/// This is used by EF Core tools to create the DbContext at design-time.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // DESIGN-TIME ONLY: This connection string is only used by EF Core tools for generating migrations.
        // At runtime, the actual connection string is provided by Aspire orchestration.
        // The password must be set via SA_PASSWORD environment variable.
        var password = Environment.GetEnvironmentVariable("SA_PASSWORD");
        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException(
                "SA_PASSWORD environment variable must be set for design-time migrations. " +
                "Example: export SA_PASSWORD=\"YourSecurePassword\"");
        }
        
        optionsBuilder.UseSqlServer($"Server=localhost;Database=lotusdb;User Id=sa;Password={password};TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
