using System.Data.Entity;
using OnlineBankingForManager.WebUI.Infrastructure;

namespace OnlineBankingForManager.WebUI
{
    public class MemberShipConfig
    {
        public static void SetInitializer()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UsersContext, MigrationConfiguration>());
            new UsersContext().UserProfiles.Find(1);
        }
    }
}