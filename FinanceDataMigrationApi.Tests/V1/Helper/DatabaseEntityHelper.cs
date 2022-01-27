using Amazon.XRay.Recorder.Core.Internal.Entities;
using AutoFixture;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;

namespace FinanceDataMigrationApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static DatabaseEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<Entity>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static DatabaseEntity CreateDatabaseEntityFrom(Entity entity)
        {
            return new DatabaseEntity
            {
                Id = int.Parse(entity.Id),
                //CreatedAt = entity.CreatedAt,
            };
        }
    }
}
