using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Runtime.Serialization;

namespace TeamAppService.Models
{
    public class User
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public DateTime date { get; set; }
        [DataMember]
        public long teamId { get; set; }

    }
}
