using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IMongoCollection<Task> Tasks
        {
            get { return database.GetCollection<Task>("Tasks"); }
        }

        public async Task<List<Task>> GetTasks(
            long? id,
            DateTime? date,
            string? title,
            int? page, 
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

            if (page.HasValue)
            {
                filter = filter & filterBuilder.Eq("page", page.Value);
            }

            if (assigneeId.HasValue)
            {
                filter = filter & filterBuilder.Eq("assigneeId", assigneeId.Value);
            }

            if (isCompleted.HasValue)
            {
                filter = filter & filterBuilder.Eq("assigneeId", isCompleted.Value);
            }

            if (!String.IsNullOrWhiteSpace(category))
            {
                filter = filter & filterBuilder.Eq("category", category);
            }

            return await Tasks.Find(filter).ToListAsync();
        }

        public async void Create(Task task)
        {
            await Tasks.InsertOneAsync(task);
        }   

        public DbSet<Task> TodoItems { get; set; }
    }
}
