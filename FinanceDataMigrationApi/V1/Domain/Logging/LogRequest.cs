using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Domain.Logging
{
    [Table("DMLogs")]
    public class LogRequest
    {
        public long id { get; set; }
        public short step_number { get; set; }
        public DateTime log_time { get; set; }
        public string class_name { get; set; }

    }
}
