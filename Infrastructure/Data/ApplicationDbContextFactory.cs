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
        // The password here is a placeholder and is not used in production.
        optionsBuilder.UseSqlServer("Server=localhost;Database=lotusdb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
