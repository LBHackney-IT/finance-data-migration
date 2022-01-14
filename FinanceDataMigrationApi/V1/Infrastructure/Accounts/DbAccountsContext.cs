using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    public class DbAccountsContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }

        /// <summary>
        /// Get or sets the Data Migration Runs
        /// </summary>
        public DbSet<DMRunLog> DMRunLogs { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Account Entities
        /// </summary>
        public DbSet<DMAccountEntity> DMAccountEntities { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAccountsContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DbAccountsContext(DbContextOptions<DbAccountsContext> options)
            : base(options)
        {
        }

        public async Task<IList<DMAccountEntity>> GetLoadedListAsync()
            => await DMAccountEntities
                .Where(x => x.IsTransformed && x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);
    }
}
