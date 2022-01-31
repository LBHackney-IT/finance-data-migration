using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using System.Linq;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class HitsGateway : IHitsGateway
    {
        private readonly DatabaseContext _databaseContext;

        public HitsGateway(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<Guid> GetLastHint()
        {
            try
            {
                var result = await _databaseContext.DmDynamoLastHInt.
                    Where(p => p.TableName.ToLower() == "tenure").
                    OrderBy(p => p.Timex).LastOrDefaultAsync().ConfigureAwait(false);

                return result?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetLastHint)}: Exception: {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}
