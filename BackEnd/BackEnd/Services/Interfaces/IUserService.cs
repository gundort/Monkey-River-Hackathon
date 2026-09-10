using System;
using BackEnd.Models;

namespace BackEnd.Services.Interfaces
{
    public interface IUserService
{
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email, string excludeUserId = null);
        Task UpdateUserAsync(string id, User user);
        Task UpdateUserPasswordAsync(string id, string newPasswordHash);
        Task UpdateUserPreferencesAsync(string id, UserPreferences preferences);
}
}


