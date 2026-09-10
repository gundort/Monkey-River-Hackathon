using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.Models
{    
    public class UserPreferences
    {
        [BsonElement("emailNotifications")]
        public bool EmailNotifications { get; set; } = true;

        [BsonElement("theme")]
        public string Theme { get; set; } = "Light"; 
    }
    
}
