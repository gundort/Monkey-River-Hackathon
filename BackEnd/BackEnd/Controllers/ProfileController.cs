using System.Security.Claims;
using BackEnd.DTOs.UserProfileDTOs;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            var profileDto = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Preferences = new UserPreferencesDto
                {
                    EmailNotifications = user.Preferences.EmailNotifications,
                    Theme = user.Preferences.Theme
                }
            };

            return Ok(profileDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (updateDto.Email != user.Email)
            {
                var emailExists = await _userService.EmailExistsAsync(updateDto.Email, userId);
                if (emailExists)
                    return BadRequest(new { message = "Email is already taken" });
            }

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.Preferences = new UserPreferences
            {
                EmailNotifications = updateDto.Preferences.EmailNotifications,
                Theme = updateDto.Preferences.Theme
            };

            await _userService.UpdateUserAsync(userId, user);

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect" });

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

            await _userService.UpdateUserPasswordAsync(userId, newPasswordHash);

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferencesDto preferencesDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            var preferences = new UserPreferences
            {
                EmailNotifications = preferencesDto.EmailNotifications,
                Theme = preferencesDto.Theme
            };

            await _userService.UpdateUserPreferencesAsync(userId, preferences);

            return Ok(new { message = "Preferences updated successfully" });
        }
    }
}
