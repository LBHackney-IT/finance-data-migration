using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Infrastructure.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<List<T>> ToListWithNoLockAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            List<T> result = default;
            using var scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                },
                TransactionScopeAsyncFlowOption.Enabled);
            result = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            scope.Complete();
            return result;
        }
    }
}
