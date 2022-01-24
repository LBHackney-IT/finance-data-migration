using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class ChargeResponse
    {
        /// <summary>
        /// Id of charge model
        /// </summary>
        /// <example>
        /// 793dd4ca-d7c4-4110-a8ff-c58eac4b90a7
        /// </example>
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the appropriate tenure
        /// </summary>
        /// <example>
        /// 793dd4ca-d7c4-4110-a8ff-c58eac4b90f8
        /// </example>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Values: [Asset, Tenure]
        /// </summary>
        /// <example>
        /// Asset
        /// </example>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Charge Group - Tenants/Leaseholders
        /// </summary>
        public ChargeGroup ChargeGroup { get; set; }
        /// <summary>
        /// Information about charges
        /// </summary>
        /// <example>
        ///     [
        ///         {
        ///             "Type":"A454",
        ///             "SubType":"a-5456",
        ///             "Frequency":"Weekly",
        ///             "Amount":1235.21,
        ///             "StartDate":2021-05-11,
        ///             "EndDate":2022-05-11
        ///         },
        ///         {
        ///             "Type":"A454",
        ///             "SubType":"a-5456",
        ///             "Frequency":"Weekly",
        ///             "Amount":1235.21,
        ///             "StartDate":2021-05-11,
        ///             "EndDate":2022-05-11
        ///         }
        ///     ]
        /// </example>
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
