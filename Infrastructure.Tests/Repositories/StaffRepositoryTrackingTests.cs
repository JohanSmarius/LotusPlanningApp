using Entities;
using Infrastructure;
using LotusPlanningApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for <see cref="StaffRepository"/> that reproduce the EF Core
/// change-tracker conflict when updating a staff member through the details page.
/// </summary>
public class StaffRepositoryTrackingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly StaffRepository _repository;

    public StaffRepositoryTrackingTests()
    {
        // In-memory SQLite connection kept open for the duration of the test.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new StaffRepository(_context);
    }

    /// <summary>
    /// Verifies that <see cref="StaffRepository.UpdateStaffAsync"/> succeeds even when
    /// the same entity is already tracked by the <see cref="ApplicationDbContext"/> from
    /// a prior <see cref="StaffRepository.GetStaffByIdAsync"/> call within the same
    /// Blazor circuit — the scenario that previously threw:
    /// "The instance of entity type 'Staff' cannot be tracked because another instance
    /// with the same key value for {'Id'} is already being tracked."
    /// </summary>
    [Fact]
    public async Task UpdateStaffAsync_WhenEntityAlreadyTrackedByContext_SuccessfullyPersistsChanges()
    {
        // Arrange: seed a staff member into the database.
        var seeded = new Staff
        {
            FirstName = "Agnes",
            LastName = "Smarius",
            Email = "agnes@mail.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateStaffAsync(seeded);

        // Simulate the page load: GetStaffByIdAsync causes EF Core to track the entity
        // in the shared (scoped) DbContext for the remainder of the request/circuit.
        var trackedEntity = await _repository.GetStaffByIdAsync(seeded.Id);
        Assert.NotNull(trackedEntity);

        // Simulate the edit form submit: a SEPARATE Staff instance built from the form
        // field values — same primary key, different object reference, one field changed.
        var editFormCopy = new Staff
        {
            Id = seeded.Id,
            FirstName = "Agnes",
            LastName = "Smarius",
            Email = "agnes@mail.com",
            Phone = "0612345678", // changed by the user
            IsActive = true,
            CreatedAt = seeded.CreatedAt
        };

        // Act: should no longer throw even though 'trackedEntity' is still tracked.
        var result = await _repository.UpdateStaffAsync(editFormCopy);

        // Assert: the returned value reflects the change.
        Assert.NotNull(result);
        Assert.Equal("0612345678", result.Phone);

        // Also verify the change was actually written to the database by reloading.
        var reloaded = await _context.Staff.AsNoTracking().FirstAsync(s => s.Id == seeded.Id);
        Assert.Equal("0612345678", reloaded.Phone);
        Assert.NotNull(reloaded.UpdatedAt);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
