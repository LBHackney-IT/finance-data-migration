using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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

        /// <summary>
        /// Extract the data migration account entities.
        /// </summary>
        /// <returns>the accounts to migrate.</returns>
        //public async Task<int> ExtractDMAccountsAsync(DateTime? processingDate)
        public async Task<int> ExtractDMAccountsAsync()
        {
            var affectedRows = await ExecuteStoredProcedure($"EXEC @returnValue = [dbo].[usp_ExtractAccountsEntity]", 600).ConfigureAwait(false);
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

        public async Task<IList<DMAccountEntity>> GetLoadedListAsync()
            => await DMAccountEntities
                .Where(x => x.IsTransformed && x.IsLoaded)
                .ToListAsync()
                .ConfigureAwait(false);
    }
}
