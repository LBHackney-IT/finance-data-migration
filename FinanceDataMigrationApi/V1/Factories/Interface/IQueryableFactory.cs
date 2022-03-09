using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Factories.Interface
{
    public interface IQueryableFactory<TObject, TQueryableObject>
    where TObject : class
    where TQueryableObject : class
    {
        public TQueryableObject ToQuaryable();
    }
}
