using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    #region MaintenanceCheckEntity
    public class MaintenanceCheckEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string Id { get; set; }

        [BsonElement("FlightRegNo"), BsonRequired]
        public string FlightRegNo { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("EngineeringTeamID")]
        public string EngineeringTeamID { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; }

        [BsonElement("TotalManHours")]
        public int TotalManHours { get; set; }
    }
    #endregion
}
