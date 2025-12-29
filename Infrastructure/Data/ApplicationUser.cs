using Microsoft.AspNetCore.Identity;

namespace LotusPlanningApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User's first name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Reference to the staff member associated with this user
        /// </summary>
        public int? StaffId { get; set; }

        /// <summary>
        /// Indicates whether the user has been approved by an admin
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// The date and time when the user was approved
        /// </summary>
        public DateTime? ApprovedAt { get; set; }

        /// <summary>
        /// The ID of the admin who approved this user
        /// </summary>
        public string? ApprovedByUserId { get; set; }

        /// <summary>
        /// The date when the user registered
        /// </summary>
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Full name property
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

}
