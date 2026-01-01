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
        
        // Use a temporary connection string for migrations
        // This will be replaced by Aspire at runtime
        optionsBuilder.UseSqlServer("Server=localhost;Database=lotusdb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
