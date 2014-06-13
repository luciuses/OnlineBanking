using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace OnlineBankingForManager.WebUI.Infrastructure
{
    public class MigrationConfiguration : DbMigrationsConfiguration<UsersContext>
    {
        public MigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;  
        }

        protected override void Seed(UsersContext context)
        {
            WebSecurity.InitializeDatabaseConnection("UsersContext", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            var role = System.Web.Security.Roles.Provider;
            if (!role.RoleExists("ActiveUser"))
            {
                role.CreateRole("ActiveUser");
            }
        }
    }
}