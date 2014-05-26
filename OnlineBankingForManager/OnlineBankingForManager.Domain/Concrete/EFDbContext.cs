using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("EFDbContext") { }
        public DbSet<Client> Clients { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}