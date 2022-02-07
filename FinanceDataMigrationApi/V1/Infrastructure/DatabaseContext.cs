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
        public DbSet<DmTransactionDbEntity> DmTransactionEntities { get; set; }

        /// <summary>
        /// Get or sets the Data Migration Charge Entity
        /// </summary>
        public DbSet<DmChargesDbEntity> ChargesDbEntities { get; set; }

        /// <summary>
        /// Get Data Migration Detailed Charges Entities
        /// </summary>
        public DbSet<DmDetailedChargesDbEntity> DetailedChargesEntities { get; set; }

        public DbSet<DmRunStatusModel> DmRunStatusModels { get; set; }
        public DbSet<DmTimeLogModel> DmTimeLogModels { get; set; }

        #region Charges Entity Specific


        /// <summary>
        /// Get the Data Migration Charge Entities
        /// </summary>
        /// <returns>The Transactions to migrate</returns>
        public async Task<IList<DmChargesDbEntity>> GetDMChargeEntitiesAsync()
            => await ChargesDbEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .ToListAsync()
                .ConfigureAwait(false);


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

        public async Task<IList<DmChargesDbEntity>> GetExtractedChargeListAsync(int count)
            => await this.Set<DmChargesDbEntity>()
                .Where(x => x.MigrationStatus == EMigrationStatus.Extracted)
                .Take(count)
                .Include(p => p.DetailedChargesDbEntities)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DmChargesDbEntity>> GetTransformedChargeListAsync(int count)
            => await this.Set<DmChargesDbEntity>()
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .Take(count)
                .Include(p => p.DetailedChargesDbEntities)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DmChargesDbEntity>> GetLoadedChargeListAsync(int count)
            => await this.Set<DmChargesDbEntity>()
                .Where(x => x.MigrationStatus == EMigrationStatus.Loaded)
                .Take(count)
                .Include(p => p.DetailedChargesDbEntities)
                .ToListAsync()
                .ConfigureAwait(false);

        ///// <summary>
        ///// Get Detailed Charge from Database Query
        ///// </summary>
        ///// <param name="paymentReference"></param>
        ///// <returns>List of Details Charges</returns>
        ///// <exception cref="Exception"></exception>
        //public async Task<List<DmDetailedChargesEntity>> GetDetailChargesListAsync(string paymentReference)
        //{

        //    var param = new SqlParameter("@payment_reference", paymentReference.TrimEnd());

        //    try
        //    {
        //        var result = await DMDetailedChargesEntities
        //            .FromSqlRaw("[dbo].[usp_ExtractDetailedChargesEntity] @payment_reference", param)
        //            .ToListAsync()
        //            .ConfigureAwait(false);

        //        return result;

        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }

        //}



        #endregion

        #region Transaction Entity Specific


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

        /// <summary>
        /// Extract the data migration transaction entities.
        /// </summary>
        /// <returns>the transactions to migrate.</returns>
        public async Task<int> ExtractDmTransactionsAsync()
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractTransactionEntity]", 6000).ConfigureAwait(false);
            return affectedRows;
        }


        public async Task<IList<DmTransactionDbEntity>> GetTransformedListAsync()
            => await DmTransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DmTransactionDbEntity>> GetLoadedListAsync()
            => await DmTransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Loaded)
                .ToListAsync()
                .ConfigureAwait(false);

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


        public async Task<IList<DmTransactionDbEntity>> GetTransformedTransactionListAsync(int count)
            => await DmTransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .Take(count)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<DmTransactionDbEntity>> GetExtractedTransactionListAsync(int count)
            => await DmTransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Extracted)
                .Take(count)
                .ToListAsync()
                .ConfigureAwait(false);

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
