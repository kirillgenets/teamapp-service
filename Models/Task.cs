using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TeamAppService.Models
{
    public class Task
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public ISet<int> Executors;
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool isCompleted { get; set; }
        [DataMember]
        public long assigneeId { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public DateTime date { get; set; }
        [DataMember]
        public int page { get; set; }

        public void AddExecutor(int id)
        {
            Executors.Add(id);
        }

        public void RemoveExecutor(int id)
        {
            Executors.Remove(id);
        }
    }
}
