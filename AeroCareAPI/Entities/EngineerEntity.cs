using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    #region EngineerEntity
    public class EngineerEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("CustomID")]
        public string CustomID { get; set; }

        [BsonElement("TeamID")]
        public string TeamID { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; }
    }
    #endregion
}
