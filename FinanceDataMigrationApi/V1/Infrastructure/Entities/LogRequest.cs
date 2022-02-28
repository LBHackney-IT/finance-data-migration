using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMLogs")]
    public class LogRequest
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("step_number")]
        public short StepNumber { get; set; }
        [Column("log_time")]
        public DateTime LogTime { get; set; }
        [Column("class_name")]
        public string ClassName { get; set; }
        [Column("proc_name")]
        public string ProcName { get; set; }
        [Column("exception_message")]
        public string ExceptionMessage { get; set; }

        public static implicit operator DbSet<object>(LogRequest v)
        {
            throw new NotImplementedException();
        }
    }
}
