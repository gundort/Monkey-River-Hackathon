using BackEnd.Controllers;
using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackendTests.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IMongoDbContext> _mockDbContext;
        private readonly Mock<IMongoCollection<User>> _mockUserCollection;
        private readonly Mock<IAsyncCursor<User>> _mockCursor;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockDbContext = new Mock<IMongoDbContext>();
            _mockUserCollection = new Mock<IMongoCollection<User>>();
            _mockCursor = new Mock<IAsyncCursor<User>>();
            _mockJwtService = new Mock<IJwtService>();

            _mockDbContext.Setup(db => db.Users).Returns(_mockUserCollection.Object);

            _controller = new AuthController(_mockDbContext.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_IfCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "validuser@example.com",
                Password = "CorrectPassword"
            };

            var storedUser = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = "Jane",
                LastName = "Doe",
                Id = "user123"
            };

            _mockCursor.Setup(_ => _.Current).Returns(new List<User> { storedUser });
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockUserCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            _mockJwtService.Setup(s => s.GenerateToken(It.IsAny<User>()))
                .Returns("valid-token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as AuthResponseDto;
            response.Should().NotBeNull();
            response.Token.Should().Be("valid-token");
        }


        [Fact]
        public async Task Register_ShouldReturnBadRequest_IfUserAlreadyExists()
        {
            // Arrange
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123" };

            var existingUser = new User { Email = dto.Email };

            _mockCursor.Setup(_ => _.Current).Returns(new List<User> { existingUser });
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockUserCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_IfUserNotFound()
        {
            // Arrange
            var dto = new LoginDto { Email = "notfound@example.com", Password = "Password123" };

            _mockCursor.Setup(_ => _.Current).Returns(new List<User>());
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUserCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_IfPasswordIsInvalid()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "validuser@example.com",
                Password = "WrongPassword"
            };

            var storedUser = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
            };

            _mockCursor.Setup(_ => _.Current).Returns(new List<User> { storedUser });
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockUserCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
        }
    }
}
