using AutoMapper;
using FinanceDataMigrationApi;
using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Threading.Tasks;

namespace AWSLambdaFunctionTester
{
    internal class Program
    {
        private readonly IMapper _autoMapper;

        public Program(IMapper mapper)
        {
            _autoMapper = mapper;
        }
        static async Task Main(string[] args)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<MigrationRun, MigrationRunDbEntity>().ReverseMap();
                cfg.CreateMap<MigrationRunUpdateRequest, MigrationRun>().ReverseMap();
            });

            IMapper mapper = config.CreateMapper();

            var h = new Handler(mapper);
            await h.ExtractTransactions().ConfigureAwait(false);

            // TODO: TBC
            //await h.TransformTransactions().ConfigureAwait(false);

            // TODO
            // await h.LoadTransactions().ConfigureAwait(false);

        }
    }
}
