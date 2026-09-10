using System;
using BackEnd.Models;

namespace BackEnd.Services.Interfaces
{
    public interface ITravelAlertService
    {
        Task<IEnumerable<TravelAlert>> GetUserAlertsAsync(string userId);
        
        Task<TravelAlert> GetUserAlertByIdAsync(string userId, string alertId);
        
        Task<TravelAlert> CreateAlertAsync(string userId, string title, string status);
        
        Task<bool> UpdateAlertAsync(string userId, string alertId, string title, string status);
        
        Task<bool> UpdateAlertStatusAsync(string userId, string alertId, string status);
        
        Task<bool> DeleteAlertAsync(string userId, string alertId);
        
        Task CreateSampleAlertsAsync(string userId);
    }
    
}

