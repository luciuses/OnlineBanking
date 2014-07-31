// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersContext.cs" company="">
//   
// </copyright>
// <summary>
//   The users context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    /// <summary>
    /// The users context.
    /// </summary>
    public class UsersContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersContext"/> class.
        /// </summary>
        public UsersContext()
            : base("UsersContext")
        {
        }

        /// <summary>
        /// Gets or sets the user profiles.
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}