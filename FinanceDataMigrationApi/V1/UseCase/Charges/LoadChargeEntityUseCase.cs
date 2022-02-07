using System;
using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges;

namespace FinanceDataMigrationApi.V1.UseCase.Charges
{
    public class LoadChargeEntityUseCase : ILoadChargeEntityUseCase
    {
        readonly int _batchSize = 25;
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IChargeGateway _chargeGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "LOAD";

        public LoadChargeEntityUseCase(IDMRunLogGateway dMRunLogGateway, IChargeGateway chargeGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _chargeGateway = chargeGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                var extractedList = await _chargeGateway.GetExtractedListAsync(count).ConfigureAwait(false);
                if (extractedList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i <= extractedList.Count / _batchSize; i++)
                    {
                        tasks.Add(_chargeGateway.BatchInsert(extractedList.OrderBy(p => p.Id).
                            Skip(i * _batchSize).Take(_batchSize).ToList()));
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Charges} Entity.");
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}" +
                    $".{nameof(Handler)}" +
                    $".{nameof(ExecuteAsync)}" +
                    $" load charge exception: {ex.Message}");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
        }
    }
}