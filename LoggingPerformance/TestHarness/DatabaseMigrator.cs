using System;
using System.Data.SqlClient;
using DbUp;
using LoggingPerformance.Octopus.Persistance;

namespace LoggingPerformance.TestHarness
{
    public class DatabaseMigrator
    {
        public void Migrate(IRelationalStore store)
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(store.ConnectionString)
                    .WithScriptsAndCodeEmbeddedInAssembly(typeof(RelationalStore).Assembly)
                    .LogScriptOutput()
                    .WithVariable("databaseName", new SqlConnectionStringBuilder(store.ConnectionString).InitialCatalog)
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Database migration failed: " + result.Error, result.Error);
            }
        }
    }
}