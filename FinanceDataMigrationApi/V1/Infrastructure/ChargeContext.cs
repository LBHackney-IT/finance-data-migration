using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public class ChargeContext: DbContext
    {
        public ChargeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ChargeDbEntity> ChargeEntities { get; set; }
    }
}
