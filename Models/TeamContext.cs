using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TeamAppService.Models;

namespace TeamAppService.Models
{
    public class TeamContext : DbContext
    {
        IMongoDatabase database;

        public TeamContext(DbContextOptions<TeamContext> options)
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

        public async System.Threading.Tasks.Task<List<Team>> GetTeams(
            long? id,
            DateTime? date
        )
        {
            var filterBuilder = new FilterDefinitionBuilder<Team>();
            var filter = filterBuilder.Empty;

            if (id.HasValue)
            {
                filter = filter & filterBuilder.Eq("id", id.Value);
            }

            if (date.HasValue)
            {
                filter = filter & filterBuilder.Eq("date", date.ToString());
            }

            return await Teams.Find(filter).ToListAsync();
        }

        public IMongoCollection<Models.Team> Teams
        {
            get { return database.GetCollection<Team>("Teams"); }
        }

        public string HashPassword(string password)
        {
            return Encoding.ASCII.GetString(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public async System.Threading.Tasks.Task Create(Team team)
        {
            team.id = Teams.Find(new FilterDefinitionBuilder<Team>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            team.date = DateTime.Now;
            team.password = HashPassword(team.password);

            await Teams.InsertOneAsync(team);
        }

        public async System.Threading.Tasks.Task Update(Team team)
        {
            team.password = HashPassword(team.password);

            await Teams.ReplaceOneAsync(new BsonDocument("id", team.id), team);
        }

        public async System.Threading.Tasks.Task Remove(long id)
        {
            await Teams.DeleteOneAsync(new BsonDocument("id", id));
        }

        public async System.Threading.Tasks.Task<Team> GetTeam(long id)
        {
            return await Teams.Find(new BsonDocument("id", id)).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task<AuthConfirmation> IsAuth(string login, string password)
        {
            Team team = await Teams.Find(new BsonDocument("login", login)).FirstOrDefaultAsync();
            AuthConfirmation confirmation = new AuthConfirmation(team.password == HashPassword(password));

            return confirmation;
        }

        public async System.Threading.Tasks.Task<bool> IsUnique(string login, long? id = null, long? teamId = null)
        {
            var filterBuilder = new FilterDefinitionBuilder<Team>();
            var filter = filterBuilder.Empty;

            // User existingUser = await Users.Find(new BsonDocument("login", login)).FirstOrDefaultAsync();

            if (id != null)
            {
                filter = filter & filterBuilder.Not(filterBuilder.Eq("id", id.Value));
            }

            if (teamId != null)
            {
                filter = filter & filterBuilder.Eq("teamId", teamId.Value);
            }

            var foundTeamsList = await Teams.Find(filter).ToListAsync();

            return foundTeamsList.Count == 0;
        }

        public DbSet<TeamAppService.Models.Team> Team { get; set; }
    }
}
