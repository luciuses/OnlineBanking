// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationConfiguration.cs" company="">
//   
// </copyright>
// <summary>
//   The migration configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure
{
    using System.Data.Entity.Migrations;
    using System.Web.Security;
    using WebMatrix.WebData;

    /// <summary>
    /// The migration configuration.
    /// </summary>
    public class MigrationConfiguration : DbMigrationsConfiguration<UsersContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationConfiguration"/> class.
        /// </summary>
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        /// <summary>
        /// The seed.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Seed(UsersContext context)
        {
            WebSecurity.InitializeDatabaseConnection("UsersContext", "UserProfile", "UserId", "UserName", true);
            RoleProvider role = Roles.Provider;
            if (!role.RoleExists("ActiveUser"))
            {
                role.CreateRole("ActiveUser");
            }
        }
    }
}