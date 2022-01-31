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
using FinanceDataMigrationApi.V1.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public sealed class DatabaseContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DMTransactionEntity>().Property(x => x.TargetId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<DetailedChargesDbEntity>()
                .HasOne(c => c.ChargesDbEntity)
                .WithMany(c => c.DetailedChargesDbEntities)
                .HasForeignKey(c => c.ChargeId);
            modelBuilder.Entity<DmDynamoTenureHouseHoldMembers>()
                .HasOne(c => c.DynamoTenure)
                .WithMany(c => c.DynamoHouseHoldMembers)
                .HasForeignKey(c => c.TenureId);
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
        public DbSet<ChargesDbEntity> ChargesDbEntities { get; set; }

        /// <summary>
        /// Get Data Migration Detailed Charges Entities
        /// </summary>
        public DbSet<DetailedChargesDbEntity> DetailedChargesEntities { get; set; }

        public DbSet<DmDynamoTenure> DynamoTenure { get; set; }
        public DbSet<DmDynamoTenureHouseHoldMembers> DynamoTenureHouseHoldMembers { get; set; }


        #region Charges Entity Specific


        /// <summary>
        /// Get the Data Migration Charge Entities
        /// </summary>
        /// <returns>The Transactions to migrate</returns>
        public async Task<IList<ChargesDbEntity>> GetDMChargeEntitiesAsync()
            => await ChargesDbEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
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
                $"EXEC @returnValue = [dbo].[usp_ExtractChargesEntity]", 60000)
                .ConfigureAwait(false);
        }

        public async Task<IList<ChargesDbEntity>> GetTransformedChargeListAsync(int count)
            => await this.Set<ChargesDbEntity>()
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .Take(count)
                .Include(p => p.DetailedChargesDbEntities)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<ChargesDbEntity>> GetLoadedChargeListAsync(int count)
            => await this.Set<ChargesDbEntity>()
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
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoAsset] '{lastHint}','{xml}'", 6000).ConfigureAwait(false);
            return affectedRows;
        }

        public async Task<int> InsertDynamoTenure(string lastHint, XElement xml)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_InsertDynamoTenure] '{lastHint}','{xml}'", 6000).ConfigureAwait(false);
            return affectedRows;
        }

        public async Task<int> SaveDynamoTenureToIFS<T>(List<T> data, string lastHint)
        {
            /*var serilizer = new XmlSerializer(typeof(DmDynamoTenure));
            var result = (DmDynamoTenure) serilizer.Deserialize(xml.CreateReader());*/

            try
            {
                /*var tenure = xml.Descendants("Tenure").Select(p => new DmDynamoTenure()
                {
                    Id = Guid.Parse(p.Element("id").Value.ToString()),
                    PaymentReference = p.Element("payment_reference")?.Value,
                    TenureTypeCode = p.Element("tenure_type_code")?.Value,
                    TenureTypeDesc = p.Element("tenure_type_desc")?.Value,
                    TenuredAssetFullAddress = p.Element("tenured_asset_full_address")?.Value,
                    TerminatedReasonCode = p.Element("terminated_reason_code")?.Value,
                    Timex = DateTime.Parse(p.Element("timex")?.Value),
                    DynamoHouseHoldMembers = p.Elements("HouseHoldMembers")?
                      .Select(d => new DmDynamoTenureHouseHoldMembers
                      {
                          Id = Guid.Parse(d.Element("id")?.Value),
                          Fullname = d.Element("fullname")?.Value,
                          IsResponsible = bool.Parse(d.Element("is_responsible")?.Value ?? "false")
                      }).ToList()
                });*/

                using (SqlConnection connection = new SqlConnection(Database.GetConnectionString()))
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    bulkCopy.DestinationTableName = ((TableAttribute) typeof(T).GetCustomAttributes(typeof(TableAttribute), false)[0]).Name;

                    DmDynamoLastHInt dmDynamoLastHInt = new DmDynamoLastHInt()
                    {
                        TableName = bulkCopy.DestinationTableName,
                        Id = Guid.Parse(lastHint),
                        Timex = DateTime.Now
                    };

                    DmDynamoLastHInt.Add(dmDynamoLastHInt);

                    await bulkCopy.WriteToServerAsync(DatatableExtension.ToDataTable(data)).ConfigureAwait(false);
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    if (data.Count == 0)
                        throw new Exception("Exception in bulk copy.");

                    SaveChanges();
                    //await Database.CommitTransactionAsync().ConfigureAwait(false);
                    return data.Count;
                }
            }
            catch (Exception ex)
            {
                //await Database.RollbackTransactionAsync().ConfigureAwait(false);
                LoggingHandler.LogError($"SqlBulkCopy Exception: " +
                                        $"{nameof(FinanceDataMigrationApi)}." +
                                        $"{nameof(Handler)}." +
                                        $"{nameof(ExecuteStoredProcedure)}:{ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Extract the data migration transaction entities.
        /// </summary>
        /// <param name="processingDate">the processiing date.</param>
        /// <returns>the transactions to migrate.</returns>
        //public async Task<int> ExtractDMTransactionsAsync(DateTime? processingDate)
        public async Task<int> ExtractDMTransactionsAsync(DateTimeOffset? processingDate)
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractTransactionEntity] '{processingDate:yyyy-MM-dd}'", 6000).ConfigureAwait(false);
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
                //await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);

                var parameterReturn = new SqlParameter
                {
                    ParameterName = "ReturnValue",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                };

                var result = await Database.ExecuteSqlRawAsync(procedureRawString, parameterReturn).ConfigureAwait(false);

                int returnValue = (int) parameterReturn.Value;
                //await Database.CommitTransactionAsync().ConfigureAwait(false);
                return returnValue;
            }
            catch (Exception exception)
            {
                //await Database.RollbackTransactionAsync().ConfigureAwait(false);
                LoggingHandler.LogError($"Executing stores procedure error in: " +
                                        $"{nameof(FinanceDataMigrationApi)}." +
                                        $"{nameof(Handler)}." +
                                        $"{nameof(ExecuteStoredProcedure)}:{exception.GetFullMessage()}");
                throw;
            }
        }


    }
}
