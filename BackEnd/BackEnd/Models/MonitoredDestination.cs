using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class MonitoredDestination
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("location")]
        [Required]
        public required string Location { get; set; }

        [BsonElement("riskLevel")]
        [Required]
        public required string RiskLevel { get; set; }

        [BsonElement("lastChecked")]
        [Required]
        public required DateTime LastChecked { get; set; }
    }
}
