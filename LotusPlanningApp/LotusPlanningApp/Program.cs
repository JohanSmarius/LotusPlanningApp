using LotusPlanningApp.Client.Pages;
using LotusPlanningApp.Components;
using LotusPlanningApp.Components.Account;
using LotusPlanningApp.Configuration;
using LotusPlanningApp.Data;
using LotusPlanningApp.Services;
using System.IO;
using Application;
using Application.Commands.StaffAssignments;
using Application.Commands.Customers;
using Application.Common;
using Entities;
using Infrastructure.Commands.StaffAssignments;
using Infrastructure.Commands.Customers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (Aspire)
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// Add controllers for API endpoints
builder.Services.AddControllers();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Add SQL Server database with Aspire
builder.AddSqlServerDbContext<ApplicationDbContext>("lotusdb");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add HttpContextAccessor for accessing HTTP context in Blazor components
builder.Services.AddHttpContextAccessor();

// Register CQRS handlers and application layer services
builder.Services.AddApplicationLayer();

// Register infrastructure services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffAssignmentRepository, StaffAssignmentRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register CQRS command handlers for staff assignments (using factory pattern to avoid contravariance issues)
builder.Services.AddScoped(typeof(ICommandHandler<LinkUserToStaffByEmailCommand, bool>), typeof(LinkUserToStaffByEmailCommandHandler));
builder.Services.AddScoped(typeof(ICommandHandler<LinkUserToStaffByIdCommand, bool>), typeof(LinkUserToStaffByIdCommandHandler));
builder.Services.AddScoped(typeof(ICommandHandler<UnlinkUserFromStaffCommand, bool>), typeof(UnlinkUserFromStaffCommandHandler));

// Register CQRS command handlers for customer assignments
builder.Services.AddScoped(typeof(ICommandHandler<LinkUserToCustomerByEmailCommand, bool>), typeof(LinkUserToCustomerByEmailCommandHandler));

// Register command dispatcher
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

// Register UI services
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Register Blazor Bootstrap
builder.Services.AddBlazorBootstrap();

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName)
);

// Register IOptions<EmailOptions> if you need to inject it directly elsewhere
builder.Services.AddOptions<EmailOptions>().Bind(builder.Configuration.GetSection(EmailOptions.SectionName));

var app = builder.Build();

// Apply migrations and seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Apply pending migrations
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
        throw;
    }
    
    await SeedRolesAndAdminAsync(services);
    await BackfillStaffForExistingUsersAsync(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LotusPlanningApp.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Map API controllers
app.MapControllers();

// Map default endpoints (health checks)
app.MapDefaultEndpoints();

app.Use(async (context, next) =>
{
    // Only set cache headers if the response hasn't started yet
    if (!context.Response.HasStarted)
    {
        context.Response.Headers["Cache-control"] = "no-cache, max-age=0, must-revalidate";
    }

    await next();
});

app.Run();

async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles if they don't exist
    string[] roles = { "Admin", "Lotus", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create admin user if not exists
    var adminEmail = "admin@lotus-tilburg.nl";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            IsApproved = true, // Admin is automatically approved
            ApprovedAt = DateTime.UtcNow,
            RegisteredAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(adminUser, "Test123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else if (!adminUser.IsApproved)
    {
        // Ensure existing admin is approved
        adminUser.IsApproved = true;
        adminUser.ApprovedAt = DateTime.UtcNow;
        await userManager.UpdateAsync(adminUser);
    }
}

/// <summary>
/// Backfills staff members for existing users who don't have staff links
/// This handles users created before the auto-linking feature was implemented
/// </summary>
async Task BackfillStaffForExistingUsersAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

    // Get all users who don't have a staff link
    var usersWithoutStaff = userManager.Users.Where(u => u.StaffId == null).ToList();

    if (usersWithoutStaff.Count == 0)
        return; // Nothing to backfill

    foreach (var user in usersWithoutStaff)
    {
        if (string.IsNullOrEmpty(user.Email))
            continue;

        // Dispatch the link command which will create a staff member if needed
        var linkCommand = new LinkUserToStaffByEmailCommand(user.Id, user.Email);
        await commandDispatcher.DispatchAsync<LinkUserToStaffByEmailCommand, bool>(linkCommand);
    }
}

