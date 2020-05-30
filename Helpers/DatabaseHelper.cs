using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.IO;

namespace TeamAppService.Helpers
{
    public class DatabaseHelper
    {
        public static IMongoDatabase GetDatabase()
        {
            string projectPath = Directory.GetCurrentDirectory();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetSection("TeamAppDatabaseSettings").GetSection("ConnectionString").Value;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);

            return client.GetDatabase(connection.DatabaseName);
        }
    }
}
