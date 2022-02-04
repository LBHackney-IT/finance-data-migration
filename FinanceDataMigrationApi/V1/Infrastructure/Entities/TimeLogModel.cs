using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Amazon.XRay.Recorder.Core.Sampling;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMTimeLog")]
    public class DmTimeLogModel
    {
        [Column("id")]
        [Key]
        public long Id { get; set; }
        [Column("proc_name")]
        public string ProcName { get; set; }
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("end_time")]
        public DateTime EndTime { get; set; }
        [Column("elapsed_time")]
        public TimeSpan ElapsedTime { get; set; }
    }
}
