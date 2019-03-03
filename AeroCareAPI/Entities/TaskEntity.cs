using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    #region TaskEntity
    public class TaskEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string Id { get; set; }

        [BsonElement("Name"), BsonRequired]
        public string Name { get; set; }

        [BsonElement("MaintenanceCheckID"), BsonRequired]
        public string MaintenanceCheckID { get; set; }

        [BsonElement("EngineerID")]
        public string EngineerID { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }
    }
    #endregion
}
