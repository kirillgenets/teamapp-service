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
    public class UserContext : DbContext
    {
        IMongoDatabase database;

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            database = DatabaseHelper.GetDatabase();
        }

        public async System.Threading.Tasks.Task<List<User>> GetUsers(
            long? id,
            DateTime? date,
            string? teamName
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

            if (!String.IsNullOrWhiteSpace(teamName))
            {
                filter = filter & filterBuilder.Eq("teamName", teamName);
            }

            return await Users.Find(filter).ToListAsync();
        }

        public IMongoCollection<Models.User> Users
        {
            get { return database.GetCollection<User>("Users"); }
        }

        public async System.Threading.Tasks.Task Create(User user)
        {
            var team = database.GetCollection<Team>("Teams").Find(new BsonDocument("id", user.teamName)).FirstOrDefaultAsync().Result;

            user.id = Users.Find(new FilterDefinitionBuilder<User>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            user.date = DateTime.Now;
            user.password = PasswordHelper.HashPassword(user.password);
            user.teamId = team.id;
            user.teamTitle = team.title;

            await Users.InsertOneAsync(user);
        }

        public async System.Threading.Tasks.Task Update(User user)
        {
            user.password = PasswordHelper.HashPassword(user.password);

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

        public async System.Threading.Tasks.Task<AuthConfirmation> IsAuth(string login, string password, string teamName)
        {
            var filterBuilder = new FilterDefinitionBuilder<User>();
            var filter = filterBuilder.Empty;

            if (!String.IsNullOrWhiteSpace(login))
            {
                filter = filter & filterBuilder.Eq("login", login);
            }

            if (!String.IsNullOrWhiteSpace(teamName))
            {
                filter = filter & filterBuilder.Eq("teamName", teamName);
            }

            User user = await Users.Find(filter).FirstOrDefaultAsync();
            AuthConfirmation confirmation = new AuthConfirmation(user != null && PasswordHelper.IsCorrect(user.password, password));

            return confirmation;
        }

        public async System.Threading.Tasks.Task<bool> IsUnique(string login, long? id = null, long? teamId = null)
        {
            var filterBuilder = new FilterDefinitionBuilder<User>();
            var filter = filterBuilder.Empty;

            if (!String.IsNullOrWhiteSpace(login))
            {
                filter = filter & filterBuilder.Eq("login", login);
            }

            if (id != null)
            {
                filter = filter & filterBuilder.Not(filterBuilder.Eq("id", id.Value));
            }

            if (teamId != null)
            {
                filter = filter & filterBuilder.Eq("teamId", teamId.Value);
            }

            var foundUsersList = await Users.Find(filter).ToListAsync();

            return foundUsersList.Count == 0;
        }

        public DbSet<TeamAppService.Models.User> User { get; set; }
    }
}
