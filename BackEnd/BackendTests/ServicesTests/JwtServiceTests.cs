using BackEnd.Models;
using BackEnd.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests.ServicesTests
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;

        public JwtServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
        }

        private void SetupConfig(string key, string issuer, string audience)
        {
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns(key);
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns(audience);
        }

        [Fact]
        public void GenerateToken_ShouldReturnToken_WhenAllSettingsProvided()
        {
            // Arrange
            var key = "superlongsecretkeywith32+chars!!";
            var issuer = "testIssuer";
            var audience = "testAudience";

            SetupConfig(key, issuer, audience);

            var service = new JwtService(_mockConfig.Object);

            var user = new User
            {
                Id = "user123",
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var token = service.GenerateToken(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            Assert.Contains(".", token);
        }

        [Fact]
        public void GenerateToken_ShouldThrow_WhenKeyMissing()
        {
            // Arrange
            SetupConfig(null, "issuer", "audience");
            var service = new JwtService(_mockConfig.Object);
            var user = new User();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
            Assert.Equal("JWT_SECRET_KEY environment variable is required.", ex.Message);
        }

        [Fact]
        public void GenerateToken_ShouldThrow_WhenIssuerMissing()
        {
            // Arrange
            SetupConfig("key", null, "audience");
            var service = new JwtService(_mockConfig.Object);
            var user = new User();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
            Assert.Equal("JWT_ISSUER environment variable is required.", ex.Message);
        }

        [Fact]
        public void GenerateToken_ShouldThrow_WhenAudienceMissing()
        {
            // Arrange
            SetupConfig("key", "issuer", null);
            var service = new JwtService(_mockConfig.Object);
            var user = new User();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
            Assert.Equal("JWT_AUDIENCE environment variable is required.", ex.Message);
        }
    }
}
