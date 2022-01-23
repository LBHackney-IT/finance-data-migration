using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.BalanceAmount).HasColumnType("decimal"); 
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.ChargedAmount).HasColumnType("decimal"); 
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.HousingBenefitAmount).HasColumnType("decimal"); 
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.PaidAmount).HasColumnType("decimal"); 
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.PeriodNo).HasColumnType("decimal"); 
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.TransactionAmount).HasColumnType("decimal");
            modelBuilder.Entity<DMTransactionEntity>().Property(x => x.TargetId).HasDefaultValueSql("NEWID()");
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

        public DbSet<DmDynamoAssetLastHInt> DmDynamoAssetLastHInt { get; set; }

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

        public async Task<int> InsertDynamoAsset(string lastHint,XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoAsset] '{lastHint}','{xml}'", 600).ConfigureAwait(false);
            return affectedRows;
        }

        /// <summary>
        /// Extract the data migration transaction entities.
        /// </summary>
        /// <param name="processingDate">the processiing date.</param>
        /// <returns>the transactions to migrate.</returns>
        //public async Task<int> ExtractDMTransactionsAsync(DateTime? processingDate)
        public async Task<int> ExtractDMTransactionsAsync(DateTimeOffset? processingDate)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractTransactionEntity] '{processingDate:yyyy-MM-dd}'", 600).ConfigureAwait(false);
            return affectedRows;
        }

        private async Task<int> ExecuteStoredProcedure(string procedureRawString, int timeout = 0)
        {
            if (timeout != 0)
                Database.SetCommandTimeout(timeout);
            try
            {
                await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);

                var parameterReturn = new SqlParameter
                {
                    ParameterName = "ReturnValue",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                };

                var result = await Database.ExecuteSqlRawAsync(procedureRawString, parameterReturn).ConfigureAwait(false);

                int returnValue = (int) parameterReturn.Value;
                await Database.CommitTransactionAsync().ConfigureAwait(false);
                return returnValue;
            }
            catch (Exception ex)
            {
                await Database.RollbackTransactionAsync().ConfigureAwait(false);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<DMTransactionEntity>> GetTransformedListAsync()
            => await DMTransactionEntities
                .Where(x => x.IsTransformed && !x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DMTransactionEntity>> GetLoadedListAsync()
            => await DMTransactionEntities
                .Where(x => x.IsTransformed && x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);
    }
}
