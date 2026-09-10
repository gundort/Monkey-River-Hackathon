using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class DiagnosticTest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        [Required]
        public required string Name { get; set; }

        [BsonElement("result")]
        [Required]
        public required string Result { get; set; }

        [BsonElement("date")]
        [Required]
        public required DateTime Date { get; set; }
    }
}
