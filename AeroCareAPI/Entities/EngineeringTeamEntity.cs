using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    #region EngineeringTeamEntity
    public class EngineeringTeamEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; }
    }
    #endregion
}
