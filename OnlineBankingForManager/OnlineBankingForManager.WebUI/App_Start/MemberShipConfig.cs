// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberShipConfig.cs" company="">
//   
// </copyright>
// <summary>
//   The member ship config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI
{
    using System.Data.Entity;
    using OnlineBankingForManager.WebUI.Infrastructure;

    /// <summary>
    /// The member ship config.
    /// </summary>
    public class MemberShipConfig
    {
        /// <summary>
        /// The set initializer.
        /// </summary>
        public static void SetInitializer()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UsersContext, MigrationConfiguration>());
            new UsersContext().UserProfiles.Find(1);
        }
    }
}