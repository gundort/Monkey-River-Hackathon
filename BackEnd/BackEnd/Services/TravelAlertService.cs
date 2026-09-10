using System;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using MongoDB.Driver;

namespace BackEnd.Services
{
    public class TravelAlertService : ITravelAlertService
    {
        private readonly MongoDbContext _mongoDbContext;

        public TravelAlertService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<IEnumerable<TravelAlert>> GetUserAlertsAsync(string userId)
        {
            return await _mongoDbContext.TravelAlerts
                .Find(a => a.UserId == userId)
                .SortByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<TravelAlert> GetUserAlertByIdAsync(string userId, string alertId)
        {
            return await _mongoDbContext.TravelAlerts
                .Find(a => a.Id == alertId && a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<TravelAlert> CreateAlertAsync(string userId, string title, string status)
        {
            var alert = new TravelAlert
            {
                UserId = userId,
                Title = title,
                Status = status,
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoDbContext.TravelAlerts.InsertOneAsync(alert);
            return alert;
        }

        public async Task<bool> UpdateAlertAsync(string userId, string alertId, string title, string status)
        {
            var filter = Builders<TravelAlert>.Filter.And(
                Builders<TravelAlert>.Filter.Eq(a => a.Id, alertId),
                Builders<TravelAlert>.Filter.Eq(a => a.UserId, userId)
            );

            var update = Builders<TravelAlert>.Update
                .Set(a => a.Title, title)
                .Set(a => a.Status, status);

            var result = await _mongoDbContext.TravelAlerts.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0;
        }

        public async Task<bool> UpdateAlertStatusAsync(string userId, string alertId, string status)
        {
            var filter = Builders<TravelAlert>.Filter.And(
                Builders<TravelAlert>.Filter.Eq(a => a.Id, alertId),
                Builders<TravelAlert>.Filter.Eq(a => a.UserId, userId)
            );

            var update = Builders<TravelAlert>.Update.Set(a => a.Status, status);

            var result = await _mongoDbContext.TravelAlerts.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0;
        }

        public async Task<bool> DeleteAlertAsync(string userId, string alertId)
        {
            var filter = Builders<TravelAlert>.Filter.And(
                Builders<TravelAlert>.Filter.Eq(a => a.Id, alertId),
                Builders<TravelAlert>.Filter.Eq(a => a.UserId, userId)
            );

            var result = await _mongoDbContext.TravelAlerts.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task CreateSampleAlertsAsync(string userId)
        {
            var sampleAlerts = new List<TravelAlert>
            {
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Hurricane warning for your Caribbean trip",
                    Status = "Active",
                    Timestamp = DateTime.UtcNow.AddMinutes(-15)
                },
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Your Paris hotel booking confirmed",
                    Status = "Resolved", 
                    Timestamp = DateTime.UtcNow.AddHours(-2)
                },
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Flight delay notification for AA123",
                    Status = "Active",
                    Timestamp = DateTime.UtcNow.AddHours(-1)
                },
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Weather advisory for your London destination",
                    Status = "Warning",
                    Timestamp = DateTime.UtcNow.AddHours(-6)
                },
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Travel insurance reminder",
                    Status = "Resolved",
                    Timestamp = DateTime.UtcNow.AddDays(-1)
                },
                new TravelAlert
                {
                    UserId = userId,
                    Title = "Passport expiration reminder",
                    Status = "Warning",
                    Timestamp = DateTime.UtcNow.AddDays(-2)
                }
            };

            await _mongoDbContext.TravelAlerts.InsertManyAsync(sampleAlerts);
        }
    }
        
}
