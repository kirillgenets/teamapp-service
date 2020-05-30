using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Runtime.Serialization;

namespace TeamAppService.Models
{
    public class Team
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public DateTime date { get; set; }
    }
}
