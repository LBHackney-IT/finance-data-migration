using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class MigrationLogDomain
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the row identifier.
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// Gets or sets the user friendly error.
        /// </summary>
        public string UserFriendlyError { get; set; }

        /// <summary>
        /// Gets or sets the application error.
        /// </summary>
        public string ApplicationError { get; set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
