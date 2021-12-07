using FinanceDataMigrationApi.V1.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DatabaseContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DMRunLog>().ToTable("DMRunLog");
            modelBuilder.Entity<DMTransactionEntity>().ToTable("DMTransactionEntity"); ;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Get or sets the Data Migration Runs
        /// </summary>
        public DbSet<DMRunLog> DMRunLogs { get; set; }


        /// <summary>
        /// Get or sets the Data Migration Transaction Entities
        /// </summary>
        public DbSet<DMTransactionEntity> DMTransactionEntities { get; set; }

        /// <summary>
        /// Get the Data Migration Transaction Entities.
        /// </summary>
        /// <returns>The Transactions to migrate.</returns>
        public async Task<IList<DMTransactionEntity>> GetDMTransactionEntitiesAsync()
            => await DMTransactionEntities
                .Where(x => x.IsTransformed == false)
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Extract the data migration transaction entities.
        /// </summary>
        /// <param name="processingDate">the processiing date.</param>
        /// <returns>the transactions to migrate.</returns>
        //public async Task<int> ExtractDMTransactionsAsync(DateTime? processingDate)
        public async Task<int> ExtractDMTransactionsAsync(DateTimeOffset? processingDate)
        {
            var affectedRows = await PerformInterpolatedTransaction($"usp_ExtractTransactionEntity {processingDate:yyyy-MM-dd}", 600).ConfigureAwait(false);
            //return affectedRows;
            return 5;

            //var sqlString = $"DECLARE	@return_value int;";
            //sqlString += $"EXEC	@return_value = usp_ExtractTransactionEntity @processingDate = '{processingDate:yyyy-MM-dd}';";
            //sqlString += $"SELECT	'Return Value' = @return_value"; 
            //var rowsAffected = await Database.ExecuteSqlRawAsync(sqlString).ConfigureAwait(false);
            //return rowsAffected;
        }

        private async Task<int> PerformInterpolatedTransaction(FormattableString sql, int timeout = 0)
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                if (timeout != 0)
                    Database.SetCommandTimeout(timeout);
                var affectedRows = await Database.ExecuteSqlInterpolatedAsync(sql).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
                return affectedRows;
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task<IList<DMTransactionEntity>> GetTransformedListAsync()
            => await DMTransactionEntities
                .Where(x => x.IsTransformed && x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);
    }
}
