using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace API.Models
{
    public class Friend
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Receiver { get; set; }
        public string Invitation { get; set; }
        public bool isActive { get; set; }
    }
}
