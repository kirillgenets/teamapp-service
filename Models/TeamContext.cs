using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TeamAppService.Helpers;
using TeamAppService.Models;

namespace TeamAppService.Models
{
    public class TeamContext : DbContext
    {
        IMongoDatabase database;

        public TeamContext(DbContextOptions<TeamContext> options)
            : base(options)
        {
            database = DatabaseHelper.GetDatabase();
        }

        public async System.Threading.Tasks.Task<List<Team>> GetTeams(
            long? id,
            DateTime? date,
            string? name,
            string? title
        )
        {
            var filterBuilder = new FilterDefinitionBuilder<Team>();
            var filter = filterBuilder.Empty;

            if (!String.IsNullOrWhiteSpace(name))
            {
                filter = filter & filterBuilder.Eq("name", name);
            }

            if (!String.IsNullOrWhiteSpace(title))
            {
                filter = filter & filterBuilder.Eq("title", title);
            }

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

        public async System.Threading.Tasks.Task Create(Team team)
        {
            team.id = Teams.Find(new FilterDefinitionBuilder<Team>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            team.date = DateTime.Now;
            team.password = PasswordHelper.HashPassword(team.password);

            await Teams.InsertOneAsync(team);
        }

        public async System.Threading.Tasks.Task Update(Team team)
        {
            team.password = PasswordHelper.HashPassword(team.password);

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
            AuthConfirmation confirmation = new AuthConfirmation(PasswordHelper.IsCorrect(team.password, password));

            return confirmation;
        }

        public async System.Threading.Tasks.Task<bool> IsUnique(string name, long? id = null)
        {
            var filterBuilder = new FilterDefinitionBuilder<Team>();
            var filter = filterBuilder.Empty;

            if (name != null)
            {
                filter = filter & filterBuilder.Eq("name", name);
            }

            if (id != null)
            {
                filter = filter & filterBuilder.Not(filterBuilder.Eq("id", id.Value));
            }

            var foundTeamsList = await Teams.Find(filter).ToListAsync();

            return foundTeamsList.Count == 0;
        }

        public DbSet<TeamAppService.Models.Team> Team { get; set; }
    }
}
