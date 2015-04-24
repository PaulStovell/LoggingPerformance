using System;
using System.Data.SqlClient;
using System.Linq;
using LoggingPerformance.Octopus.Persistance;
using ProtoBuf.Meta;

namespace LoggingPerformance.TestHarness
{
    public static class IntegrationTestDatabase
    {
        static readonly string TestDatabaseName;
        static readonly string TestDatabaseConnectionString;
        
        static IntegrationTestDatabase()
        {
            RuntimeTypeModel.Default.AllowParseableTypes = true;

            TestDatabaseName = "OctopusDeploy-LoggingPerformanceTest";

            TestDatabaseConnectionString = string.Format("Server={0};Database={1};Trusted_connection=true", "(local)\\SQLEXPRESS", TestDatabaseName);

            DropDatabase();
            CreateDatabase();

            InitializeStore();
            InstallSchema();
        }

        public static RelationalStore Store { get; set; }
        public static RelationalMappings Mappings { get; set; }

        static void CreateDatabase()
        {
            ExecuteScript(@"create database [" + TestDatabaseName + "]", GetMaster(TestDatabaseConnectionString));
        }

        static void DropDatabase()
        {
            try
            {
                Console.WriteLine("Connecting to the 'master' database at " + TestDatabaseConnectionString);
                Console.WriteLine("Dropping " + TestDatabaseName);
                ExecuteScript("ALTER DATABASE [" + TestDatabaseName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; drop database [" + TestDatabaseName + "]", GetMaster(TestDatabaseConnectionString));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not drop the existing database: " + ex.Message);
            }
        }

        static RelationalMappings DiscoverMappings()
        {
            var mappers = (
                from type in typeof(IntegrationTestDatabase).Assembly.GetTypes()
                where typeof(DocumentMap).IsAssignableFrom(type)
                where type.IsClass && !type.IsAbstract
                select Activator.CreateInstance(type) as DocumentMap).ToList();

            var mappings = new RelationalMappings();
            mappings.Install(mappers);
            return mappings;
        }

        static void InitializeStore()
        {
            Mappings = DiscoverMappings();
            Store = new RelationalStore(TestDatabaseConnectionString, Mappings);
        }

        static void InstallSchema()
        {
            Console.WriteLine("Performing migration");
            var migrator = new DatabaseMigrator();
            migrator.Migrate(Store);
        }

        public static void ExecuteScript(string script, string connectionString = null)
        {
            using (var connection = new SqlConnection(connectionString ?? TestDatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static string GetMaster(string sqlConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(sqlConnectionString);
            builder.InitialCatalog = "master";
            return builder.ToString();
        }
    }
}