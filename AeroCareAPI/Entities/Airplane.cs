using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class Airplane
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("FlyingHours")]
        public int FlyingHours { get; set; }

        [BsonElement("Miles")]
        public int Miles { get; set; }

        [BsonElement("RegNo")]
        public string RegNo { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; }
    }
}
