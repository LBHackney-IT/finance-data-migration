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
            modelBuilder.Entity<DMDetailedChargesEntity>().HasNoKey();
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

        public DbSet<DmDynamoLastHInt> DmDynamoLastHInt { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Transaction Entities
        /// </summary>
        public DbSet<DMTransactionEntity> DMTransactionEntities { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Charge Entity
        /// </summary>
        public DbSet<DMChargesEntity> DMChargeEntities { get; set; }

        /// <summary>
        /// Get Data Migration Detailed Charges Entities
        /// </summary>
        public DbSet<DMDetailedChargesEntity> DMDetailedChargesEntities { get; set; }


        #region Charges Entity Specific


        /// <summary>
        /// Get the Data Migration Charge Entities
        /// </summary>
        /// <returns>The Transactions to migrate</returns>
        public async Task<IList<DMChargesEntity>> GetDMChargeEntitiesAsync()
            => await DMChargeEntities
                .Where(x => x.IsTransformed == false)
                .ToListAsync()
                .ConfigureAwait(false);


        /// <summary>
        /// Extract the data migration charges entities
        /// </summary>
        /// <returns>the charges to migrate</returns>
        public async Task<int> ExtractDMChargesAsync()
        {
            //TODO: StoredProc does not have processingDate parameters, need to clarify with Felipe, keep it consistent

            return await ExecuteStoredProcedure(
                $"EXEC @returnValue = [dbo].[usp_ExtractChargesEntity]", 600)
                .ConfigureAwait(false);
        }

        public async Task<IList<DMChargesEntity>> GetTransformedChargeListAsync()
            => await DMChargeEntities
                .Where(x => x.IsTransformed && !x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DMChargesEntity>> GetLoadedChargeListAsync()
            => await DMChargeEntities
                .Where(x => x.IsTransformed && x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Get Detailed Charge from Database Query
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <returns>List of Details Charges</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<DMDetailedChargesEntity>> GetDetailChargesListAsync(string paymentReference)
        {

            var param = new SqlParameter("@payment_reference", paymentReference.TrimEnd());

            try
            {
                var result = await DMDetailedChargesEntities
                    .FromSqlRaw("[dbo].[usp_ExtractDetailedChargesEntity] @payment_reference", param)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return result;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }



        #endregion

        #region Transaction Entity Specific


        /// <summary>
        /// Get the Data Migration Transaction Entities.
        /// </summary>
        /// <returns>The Transactions to migrate.</returns>
        public async Task<IList<DMTransactionEntity>> GetDMTransactionEntitiesAsync()
            => await DMTransactionEntities
                .Where(x => x.IsTransformed == false)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<int> InsertDynamoAsset(string lastHint, XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoAsset] '{lastHint}','{xml}'", 600).ConfigureAwait(false);
            return affectedRows;
        }

        public async Task<int> InsertDynamoTenure(string lastHint, XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoTenure] '{lastHint}','{xml}'", 600).ConfigureAwait(false);
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

        #endregion
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


    }
}