using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Domain.Profiles
{
    /// <summary>
    /// The migration run mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class MigrationRunMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationRunMappingProfile"/> class.
        /// </summary>
        public MigrationRunMappingProfile()
        {
            CreateMap<DMRunLogDomain, DMRunLog>().ReverseMap();
            CreateMap<MigrationRunUpdateRequest, DMRunLogDomain>().ReverseMap();
        }
    }
}
