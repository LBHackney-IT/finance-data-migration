namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class GetAllAssetListResponse : GetAssetListResponse
    {
        private string _lastHitId;

        public void SetLastHitId(string lastHitId)
        {
            _lastHitId = lastHitId;
        }

        public string LastHitId()
        {
            return _lastHitId;
        }
    }
}
