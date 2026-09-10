using BackEnd.Models;
using BackEnd.Services.Interfaces;
using MongoDB.Driver;

namespace BackEnd.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbContext _mongoDbContext;

        public UserService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _mongoDbContext.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _mongoDbContext.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, string excludeUserId = null)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq(u => u.Email, email);

            if (!string.IsNullOrEmpty(excludeUserId))
            {
                var excludeFilter = filterBuilder.Ne(u => u.Id, excludeUserId);
                filter = filterBuilder.And(filter, excludeFilter);
            }

            var existingUser = await _mongoDbContext.Users
                .Find(filter)
                .FirstOrDefaultAsync();
            
            return existingUser != null;
        }

        public async Task UpdateUserAsync(string id, User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _mongoDbContext.Users.ReplaceOneAsync(filter, user);
        }

        public async Task UpdateUserPasswordAsync(string id, string newPasswordHash)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, newPasswordHash)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _mongoDbContext.Users.UpdateOneAsync(filter, update);
        }
        
        public async Task UpdateUserPreferencesAsync(string id, UserPreferences preferences)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.Preferences, preferences)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _mongoDbContext.Users.UpdateOneAsync(filter, update);
        }
    }
}