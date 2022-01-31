using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureGateway
    {
        public Task<List<TenureInformation>> GetByPrnAsync(string prn);
        public Task<List<TenureInformation>> GetByPrnAsync(List<string> prn);
        public Task<TenureInformation> GetByIdAsync(Guid id);
        public Task<bool> BatchInsert(List<TenureInformation> tenures);
        public Task<TenurePaginationResponse> GetAll(int count, Dictionary<string, AttributeValue> lastEvaluatedKey);
        public Task<int> SaveTenuresIntoSql(string lastHint, XElement xml);
        public Task<Guid> GetLastHint();
    }
}
