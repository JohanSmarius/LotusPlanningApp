using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests;

public class ApplicationDbContextIdentityModelTests
{
    [Fact]
    public void Model_preserves_identity_login_and_token_key_lengths_and_passkeys()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LotusPlanningApp_Tests;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        using var context = new ApplicationDbContext(options);

        var loginEntity = context.Model.FindEntityType(typeof(IdentityUserLogin<string>));
        Assert.NotNull(loginEntity);
        Assert.Equal(128, loginEntity!.FindProperty(nameof(IdentityUserLogin<string>.LoginProvider))!.GetMaxLength());
        Assert.Equal(128, loginEntity.FindProperty(nameof(IdentityUserLogin<string>.ProviderKey))!.GetMaxLength());

        var tokenEntity = context.Model.FindEntityType(typeof(IdentityUserToken<string>));
        Assert.NotNull(tokenEntity);
        Assert.Equal(128, tokenEntity!.FindProperty(nameof(IdentityUserToken<string>.LoginProvider))!.GetMaxLength());
        Assert.Equal(128, tokenEntity.FindProperty(nameof(IdentityUserToken<string>.Name))!.GetMaxLength());

        var passkeyEntity = context.Model.FindEntityType(typeof(IdentityUserPasskey<string>));
        Assert.NotNull(passkeyEntity);
        Assert.Equal("AspNetUserPasskeys", passkeyEntity!.GetTableName());
    }
}
