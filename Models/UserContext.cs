using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.IO;

namespace TeamAppService.Models
{
    public class UserContext : DbContext
    {
        IMongoDatabase database;

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            string projectPath = Directory.GetCurrentDirectory();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetSection("TeamAppDatabaseSettings").GetSection("ConnectionString").Value;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);

            database = client.GetDatabase(connection.DatabaseName);
        }
    }
}
