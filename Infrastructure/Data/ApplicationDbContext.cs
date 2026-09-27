using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LotusPlanningApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<
            ApplicationUser,
            IdentityRole,
            string,
            IdentityUserClaim<string>,
            IdentityUserRole<string>,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>,
            IdentityUserPasskey<string>>(options)
    {
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Staff> Staff { get; set; } = null!;
        public DbSet<StaffAssignment> StaffAssignments { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Event entity
            builder.Entity<Event>()
                .HasMany(e => e.Shifts)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Shift entity
            builder.Entity<Shift>()
                .HasMany(s => s.StaffAssignments)
                .WithOne(sa => sa.Shift)
                .HasForeignKey(sa => sa.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Staff entity
            builder.Entity<Staff>()
                .HasMany(s => s.StaffAssignments)
                .WithOne(sa => sa.Staff)
                .HasForeignKey(sa => sa.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            builder.Entity<Event>()
                .HasIndex(e => e.StartDate);

            builder.Entity<Shift>()
                .HasIndex(s => s.StartTime);

            builder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // Configure Customer entity
            builder.Entity<Customer>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure index for Customer email
            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<IdentityUserPasskey<string>>(b =>
            {
                b.ToTable("AspNetUserPasskeys");
                b.HasKey(passkey => passkey.CredentialId);
                b.Property(passkey => passkey.CredentialId).HasMaxLength(1024);
                b.Property(passkey => passkey.UserId).IsRequired();
                b.HasIndex(passkey => passkey.UserId);
                b.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(passkey => passkey.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.OwnsOne(passkey => passkey.Data, passkeyData =>
                {
                    passkeyData.ToTable("AspNetUserPasskeys");
                    passkeyData.ToJson("Data");
                    passkeyData.WithOwner()
                        .HasForeignKey("IdentityUserPasskeyCredentialId");
                    passkeyData.Property<byte[]>("IdentityUserPasskeyCredentialId");
                    passkeyData.HasKey("IdentityUserPasskeyCredentialId");
                    passkeyData.Property(data => data.PublicKey).IsRequired();
                    passkeyData.Property(data => data.AttestationObject).IsRequired();
                    passkeyData.Property(data => data.ClientDataJson).IsRequired();
                    passkeyData.PrimitiveCollection(data => data.Transports);
                });

                b.Navigation(passkey => passkey.Data).IsRequired();
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(login => login.LoginProvider).HasMaxLength(128);
                b.Property(login => login.ProviderKey).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(token => token.LoginProvider).HasMaxLength(128);
                b.Property(token => token.Name).HasMaxLength(128);
            });
        }
    }
}
