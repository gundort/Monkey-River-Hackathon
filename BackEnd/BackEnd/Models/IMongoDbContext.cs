using MongoDB.Driver;

namespace BackEnd.Models
{
    public interface IMongoDbContext
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<DiagnosticTest> DiagnosticTests { get; }
        IMongoCollection<MonitoredDestination> MonitoredDestinations { get; }
        IMongoCollection<TravelAlert> TravelAlerts { get; }
    }

}
