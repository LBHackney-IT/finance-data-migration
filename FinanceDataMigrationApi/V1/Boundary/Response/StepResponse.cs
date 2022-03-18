using System;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class StepResponse
    {
        public bool Continue { get; set; }
        public DateTime NextStepTime { get; set; }
        public string NextLambda { get; set; }  
    }
}
