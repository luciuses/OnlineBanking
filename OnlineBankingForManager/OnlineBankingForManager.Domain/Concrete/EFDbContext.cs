// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EFDbContext.cs" company="">
//   
// </copyright>
// <summary>
//   The ef db context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.Domain.Concrete
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using OnlineBankingForManager.Domain.Entities;

    /// <summary>
    /// The ef db context.
    /// </summary>
    public class EFDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFDbContext"/> class.
        /// </summary>
        public EFDbContext()
            : base("EFDbContext")
        {
        }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        public DbSet<Client> Clients { get; set; }

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