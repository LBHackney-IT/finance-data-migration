using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class Person
    {
        // [NonEmptyGuid("PersonId")]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
