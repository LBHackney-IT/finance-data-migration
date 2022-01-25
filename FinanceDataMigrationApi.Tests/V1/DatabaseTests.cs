using FinanceDataMigrationApi.Tests;
using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace BaseApi.Tests
{
    public class DatabaseTests : IDisposable
    {
        private IDbContextTransaction _transaction;
        protected DatabaseContext DatabaseContext { get; private set; }

        public DatabaseTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(ConnectionString.TestDatabase());
            DatabaseContext = new DatabaseContext(builder.Options);
            DatabaseContext.Database.EnsureCreated();
            _transaction = DatabaseContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            // Dispose(true);
            // GC.SuppressFinalize(this);
        }

    }
}
