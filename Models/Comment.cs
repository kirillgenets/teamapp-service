using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Runtime.Serialization;

namespace TeamAppService.Models
{
    public class Comment
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public long authorId { get; set; }
        [DataMember]
        public string authorLogin { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public long taskId { get; set; }
        [DataMember]
        public long teamId { get; set; }
        [DataMember]
        public DateTime date { get; set; }
    }
}
