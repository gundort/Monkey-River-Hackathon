using BackEnd.Controllers;
using BackEnd.DTOs.UserProfileDTOs;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests.ControllerTests
{
    public class ProfileControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly ProfileController _controller;
        private readonly string _userId = "user-123";

        public ProfileControllerTests()
        {
            _mockUserService = new Mock<IUserService>();

            _controller = new ProfileController(_mockUserService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetProfile_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = _userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow,
                Preferences = new UserPreferences { EmailNotifications = true, Theme = "dark" }
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);

            var profileDto = okResult.Value as UserProfileDto;
            profileDto.Should().NotBeNull();
            profileDto.Email.Should().Be(user.Email);
            profileDto.FullName.Should().Be(user.FullName);
            profileDto.Preferences.EmailNotifications.Should().BeTrue();
            profileDto.Preferences.Theme.Should().Be("dark");
        }

        [Fact]
        public async Task GetProfile_ShouldReturnUnauthorized_WhenUserIdMissing()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized.Should().NotBeNull();
            unauthorized.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var dto = new UpdateProfileDto();

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnBadRequest_WhenEmailAlreadyTaken()
        {
            // Arrange
            var dto = new UpdateProfileDto
            {
                Email = "taken@example.com",
                FirstName = "John",
                LastName = "Doe",
                Preferences = new UserPreferencesDto { EmailNotifications = true, Theme = "light" }
            };

            var existingUser = new User
            {
                Id = _userId,
                Email = "old@example.com",
                Preferences = new UserPreferences { EmailNotifications = false, Theme = "dark" }
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(existingUser);
            _mockUserService.Setup(x => x.EmailExistsAsync(dto.Email, _userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnOk_WhenProfileUpdated()
        {
            // Arrange
            var dto = new UpdateProfileDto
            {
                Email = "new@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Preferences = new UserPreferencesDto { EmailNotifications = true, Theme = "light" }
            };

            var existingUser = new User
            {
                Id = _userId,
                Email = "old@example.com",
                Preferences = new UserPreferences { EmailNotifications = false, Theme = "dark" }
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(existingUser);
            _mockUserService.Setup(x => x.EmailExistsAsync(dto.Email, _userId))
                .ReturnsAsync(false);
            _mockUserService.Setup(x => x.UpdateUserAsync(_userId, It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "wrongpass",
                NewPassword = "newpassword123"
            };

            var user = new User
            {
                Id = _userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass")
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenPasswordChanged()
        {
            // Arrange
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "correctpass",
                NewPassword = "newpassword123"
            };

            var user = new User
            {
                Id = _userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass")
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);
            _mockUserService.Setup(x => x.UpdateUserPasswordAsync(_userId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdatePreferences_ShouldReturnOk_WhenPreferencesUpdated()
        {
            // Arrange
            var dto = new UserPreferencesDto
            {
                EmailNotifications = true,
                Theme = "dark"
            };

            var user = new User
            {
                Id = _userId,
                Preferences = new UserPreferences { EmailNotifications = false, Theme = "light" }
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);
            _mockUserService.Setup(x => x.UpdateUserPreferencesAsync(_userId, It.IsAny<UserPreferences>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePreferences(dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }
    }
}
