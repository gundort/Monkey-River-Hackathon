using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackEnd.Models
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase Database => _database;
        public IMongoCollection<DiagnosticTest> DiagnosticTests => _database.GetCollection<DiagnosticTest>("DiagnosticTests");
        public IMongoCollection<MonitoredDestination> MonitoredDestinations => _database.GetCollection<MonitoredDestination>("MonitoredDestinations");
        public IMongoCollection<TravelAlert> TravelAlerts => _database.GetCollection<TravelAlert>("travelAlerts");

        public IMongoCollection<User> Users()
        {
            return _database.GetCollection<User>("users");
        }
        public async Task<bool> PingAsync()
        {
            try
            {
                var result = await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                return result != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
