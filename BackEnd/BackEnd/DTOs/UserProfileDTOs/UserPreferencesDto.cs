using System;

namespace BackEnd.DTOs.UserProfileDTOs;

public class UserPreferencesDto
{
    public bool EmailNotifications { get; set; }
    public string Theme { get; set; } = string.Empty;
}
