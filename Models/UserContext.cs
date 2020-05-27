using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public async System.Threading.Tasks.Task<List<User>> GetTasks(
            long? id,
            DateTime? date
        )
        {
            var filterBuilder = new FilterDefinitionBuilder<User>();
            var filter = filterBuilder.Empty;

            if (id.HasValue)
            {
                filter = filter & filterBuilder.Eq("id", id.Value);
            }

            if (date.HasValue)
            {
                filter = filter & filterBuilder.Eq("date", date.ToString());
            }

            return await Users.Find(filter).ToListAsync();
        }

        public IMongoCollection<Models.User> Users
        {
            get { return database.GetCollection<User>("Users"); }
        }

        public string hashPassword(string password)
        {
            return Encoding.ASCII.GetString(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public async System.Threading.Tasks.Task Create(User user)
        {
            user.id = Users.Find(new FilterDefinitionBuilder<User>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            user.date = DateTime.Now;
            user.password = hashPassword(user.password);

            await Users.InsertOneAsync(user);
        }

        public async System.Threading.Tasks.Task Update(User user)
        {
            await Users.ReplaceOneAsync(new BsonDocument("id", user.id), user);
        }

        public async System.Threading.Tasks.Task Remove(long id)
        {
            await Users.DeleteOneAsync(new BsonDocument("id", id));
        }

        public async System.Threading.Tasks.Task<User> GetUser(long id)
        {
            return await Users.Find(new BsonDocument("id", id)).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task<AuthConfirmation> IsAuth(string login, string password)
        {
            User user = await Users.Find(new BsonDocument("login", login)).FirstOrDefaultAsync();
            AuthConfirmation confirmation = new AuthConfirmation(hashPassword(user.password) == hashPassword(password));

            return confirmation;
        }
    }
}
