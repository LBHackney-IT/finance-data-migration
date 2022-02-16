using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using System.Threading;
using System.Transactions;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    /// <summary>
    /// The database context class to work with transaction entities.
    /// </summary>
    /// <seealso cref="DbContext" />
    public sealed class DatabaseContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.BalanceAmount).HasColumnType("decimal");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.ChargedAmount).HasColumnType("decimal");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.HousingBenefitAmount).HasColumnType("decimal");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.PaidAmount).HasColumnType("decimal");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.PeriodNo).HasColumnType("decimal");
            //modelBuilder.Entity<DMTransactionEntity>().Property(x => x.TransactionAmount).HasColumnType("decimal");
            modelBuilder.Entity<DmTransactionDbEntity>().Property(x => x.TargetId).HasDefaultValueSql("NEWID()");
            //modelBuilder.Entity<ChargesDbEntity>()
            //    .HasMany(c => c.DetailedChargesDbEntities)
            //    .WithOne(d => d.ChargesDbEntity)
            //    .HasForeignKey(c => c.ChargeId);
            modelBuilder.Entity<DmDetailedChargesDbEntity>()
                .HasOne(c => c.ChargesDbEntity)
                .WithMany(c => c.DetailedChargesDbEntities)
                .HasForeignKey(c => c.ChargeId);

            modelBuilder.Entity<DmConsolidatedChargeDbEntity>()
                .HasOne(c => c.AccountDbEntity)
                .WithMany(c => c.ConsolidatedCharges)
                .HasForeignKey(c => c.AccountId);

            modelBuilder.Entity<DmTenureDbEntity>()
                .HasOne(c => c.AccountDbEntity)
                .WithOne(c => c.Tenure)
                .HasForeignKey<DmAccountDbEntity>(a => a.TargetId);

            modelBuilder.Entity<DMPrimaryTenantsDbEntity>()
                .HasOne(c => c.TenureDbEntity)
                .WithMany(p => p.PrimaryTenants)
                .HasForeignKey(c => c.TenureId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Get or sets the Data Migration Runs
        /// </summary>
        public DbSet<DMRunLog> DMRunLogs { get; set; }

        public DbSet<DmDynamoLastHInt> DmDynamoLastHInt { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Transaction Entities
        /// </summary>
        public DbSet<DmTransactionDbEntity> TransactionEntities { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Charge Entity
        /// </summary>
        public DbSet<DmChargesDbEntity> ChargesDbEntities { get; set; }

        /// <summary>
        /// Get Data Migration Detailed Charges Entities
        /// </summary>
        public DbSet<DmDetailedChargesDbEntity> DetailedChargesEntities { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Account Entities
        /// </summary>
        public DbSet<DmAccountDbEntity> AccountDbEntities { get; set; }
        public DbSet<DmConsolidatedChargeDbEntity> ConsolidatedChargeDbEntities { get; set; }

        public DbSet<DmRunStatusModel> DmRunStatusModels { get; set; }
        public DbSet<DmTimeLogModel> DmTimeLogModels { get; set; }

        #region Charges Entity Specific
        /// <summary>
        /// Extract the data migration charges entities
        /// </summary>
        /// <returns>the charges to migrate</returns>
        public async Task<int> ExtractDmChargesAsync()
        {
            return await ExecuteStoredProcedure(
                $"EXEC @returnValue = [dbo].[usp_ExtractChargesEntity]", 60000)
                .ConfigureAwait(false);
        }


        #endregion

        #region Asset & Tenure
        public async Task<int> InsertDynamoAsset(string lastHint, XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoAsset] '{lastHint}','{xml}'", 6000).ConfigureAwait(false);
            return affectedRows;
        }

        public async Task<int> InsertDynamoTenure(string lastHint, XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoTenure] '{lastHint}','{xml}'", 6000).ConfigureAwait(false);
            return affectedRows;
        }
        #endregion

        #region Transaction Entity Specific
        /// <summary>
        /// Extract the data migration transaction entities.
        /// </summary>
        /// <returns>the transactions to migrate.</returns>
        public async Task<int> ExtractDmTransactionsAsync()
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractTransactionEntity]", 6000).ConfigureAwait(false);
            return affectedRows;
        }

        public async Task<IList<DmTransactionDbEntity>> GetLoadedTransactionListAsync(int count)
            => await TransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Loaded)
                .Take(count)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

        public async Task<IList<DmTransactionDbEntity>> GetToBeDeletedTransactionListAsync(int count)
            => await TransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.ToBeDeleted)
                .Take(count)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

        #endregion

        #region Account
        /// <summary>
        /// Extract the data migration account entities.
        /// </summary>
        /// <returns>the accounts to migrate.</returns>
        public async Task<int> ExtractDmAccountsAsync()
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractAccountsEntity]", 600).ConfigureAwait(false);
            return affectedRows;
        }

        #endregion

        private async Task<int> ExecuteStoredProcedure(string procedureRawString, int timeout = 0)
        {
            if (timeout != 0)
                Database.SetCommandTimeout(timeout);
            try
            {
                var parameterReturn = new SqlParameter
                {
                    ParameterName = "ReturnValue",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                };

                var result = await Database.ExecuteSqlRawAsync(procedureRawString, parameterReturn).ConfigureAwait(false);

                int returnValue = (int) parameterReturn.Value;

                return returnValue;
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"Executing stores procedure error in: " +
                                        $"{nameof(FinanceDataMigrationApi)}." +
                                        $"{nameof(Handler)}." +
                                        $"{nameof(ExecuteStoredProcedure)}:{exception.GetFullMessage()}");
                throw;
            }
        }

        private async Task<int> ExecuteStoredProcedureWithReturnsResultSet(string procedureRawString, int timeout = 0)
        {
            if (timeout != 0)
                Database.SetCommandTimeout(timeout);
            try
            {
                var parameterReturn = new SqlParameter
                {
                    ParameterName = "ReturnValue",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                };

                var result = await Database.ExecuteSqlRawAsync(procedureRawString, parameterReturn).ConfigureAwait(false);

                int returnValue = (int) parameterReturn.Value;

                return returnValue;
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"Executing stores procedure error in: " +
                                        $"{nameof(FinanceDataMigrationApi)}." +
                                        $"{nameof(Handler)}." +
                                        $"{nameof(ExecuteStoredProcedure)}:{exception.GetFullMessage()}");
                throw;
            }
        }



        public static DatabaseContext Create()
        {
            DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
            else
                throw new Exception($"Connection string is null.");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
