using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TeamAppService.Models
{
    public class TaskContext : DbContext 
    {
        IMongoDatabase database;
        IGridFSBucket gridFS;

        public TaskContext(DbContextOptions<TaskContext> options)
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
            gridFS = new GridFSBucket(database);
        }

        public IMongoCollection<Models.Task> Tasks
        {
            get { return database.GetCollection<Task>("Tasks"); }
        }

        public async Task<List<Models.Task>> GetTasks(
            long? id,
            DateTime? date,
            string? title,
            string? category,
            long? assigneeId,
            bool? isCompleted
        )
        {
            var filterBuilder = new FilterDefinitionBuilder<Task>();
            var filter = filterBuilder.Empty;

            if (!String.IsNullOrWhiteSpace(title))
            {
                filter = filter & filterBuilder.Regex("title", new BsonRegularExpression(title));
            }

            if (id.HasValue)
            {
                filter = filter & filterBuilder.Eq("id", id.Value);
            }

            if (date.HasValue)
            {
                filter = filter & filterBuilder.Eq("date", date.ToString());
            }

            if (assigneeId.HasValue)
            {
                filter = filter & filterBuilder.Eq("assigneeId", assigneeId.Value);
            }

            if (isCompleted.HasValue)
            {
                filter = filter & filterBuilder.Eq("isCompleted", isCompleted.Value);
            }

            if (!String.IsNullOrWhiteSpace(category))
            {
                filter = filter & filterBuilder.Eq("category", category);
            }

            return await Tasks.Find(filter).ToListAsync();
        }

        public async Task<Models.Task> GetTask(long id)
        {
            return await Tasks.Find(new BsonDocument("id", id)).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task Create(Task task)
        {
            task.id = Tasks.Find(new FilterDefinitionBuilder<Task>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            task.date = DateTime.Now;

            await Tasks.InsertOneAsync(task);
        }

        public async System.Threading.Tasks.Task Update(Task task)
        {
            await Tasks.ReplaceOneAsync(new BsonDocument("id", task.id), task);
        }

        public async System.Threading.Tasks.Task Remove(long id)
        {
            await Tasks.DeleteOneAsync(new BsonDocument("id", id));
        }

        public DbSet<Task> TodoItems { get; set; }
    }
}
