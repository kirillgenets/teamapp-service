using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TeamAppService.Helpers;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class CommentContext : DbContext
    {
        IMongoDatabase database;

        public CommentContext(DbContextOptions<CommentContext> options)
            : base(options)
        {
            database = DatabaseHelper.GetDatabase();
        }

        public IMongoCollection<Models.Comment> Comments
        {
            get { return database.GetCollection<Comment>("Comments"); }
        }

        public async Task<List<Models.Comment>> GetComments(
            long? id,
            DateTime? date,
            string? text,
            long? authorId,
            string? authorName,
            long? teamId
        )
        {
            var filterBuilder = new FilterDefinitionBuilder<Comment>();
            var filter = filterBuilder.Empty;

            if (!String.IsNullOrWhiteSpace(text))
            {
                filter = filter & filterBuilder.Eq("text", text);
            }

            if (id.HasValue)
            {
                filter = filter & filterBuilder.Eq("id", id.Value);
            }

            if (date.HasValue)
            {
                filter = filter & filterBuilder.Eq("date", date.ToString());
            }

            if (authorId.HasValue)
            {
                filter = filter & filterBuilder.Eq("authorId", authorId.Value);
            }

            if (!String.IsNullOrWhiteSpace(authorName))
            {
                filter = filter & filterBuilder.Eq("authorName", authorName);
            }

            if (teamId.HasValue)
            {
                filter = filter & filterBuilder.Eq("teamId", teamId.Value);
            }

            return await Comments.Find(filter).ToListAsync();
        }

        public async Task<Models.Comment> GetComment(long id)
        {
            return await Comments.Find(new BsonDocument("id", id)).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task Create(Comment comment)
        {
            var author = database.GetCollection<User>("Users").Find(new BsonDocument("id", comment.authorId)).FirstOrDefaultAsync().Result;

            comment.id = Comments.Find(new FilterDefinitionBuilder<Comment>().Empty).ToList().Count; // auto-increment (each new item has an id equal to the items count)
            comment.date = DateTime.Now;
            comment.authorLogin = author.login;

            await Comments.InsertOneAsync(comment);
        }

        public async System.Threading.Tasks.Task Update(Comment comment)
        {
            await Comments.ReplaceOneAsync(new BsonDocument("id", comment.id), comment);
        }

        public async System.Threading.Tasks.Task Remove(long id)
        {
            await Comments.DeleteOneAsync(new BsonDocument("id", id));
        }

        public DbSet<Task> TodoItems { get; set; }
    }
}
