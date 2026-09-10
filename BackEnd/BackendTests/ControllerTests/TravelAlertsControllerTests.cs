using BackEnd.Controllers;
using BackEnd.DTOs.TravelAlertDTOs;
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
    public class TravelAlertsControllerTests
    {
        private readonly Mock<ITravelAlertService> _mockService;
        private readonly TravelAlertsController _controller;
        private readonly string _userId = "user-123";

        public TravelAlertsControllerTests()
        {
            _mockService = new Mock<ITravelAlertService>();

            _controller = new TravelAlertsController(_mockService.Object);

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
        public async Task GetTravelAlerts_ShouldReturnOkWithAlerts()
        {
            // Arrange
            var alerts = new List<TravelAlert>
            {
                new TravelAlert { Id = "1", Title = "Alert1", Status = "Active", Timestamp = System.DateTime.UtcNow },
                new TravelAlert { Id = "2", Title = "Alert2", Status = "Inactive", Timestamp = System.DateTime.UtcNow }
            };
            _mockService.Setup(s => s.GetUserAlertsAsync(_userId)).ReturnsAsync(alerts);

            // Act
            var result = await _controller.GetTravelAlerts();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);

            var dtos = okResult.Value as List<TravelAlertDto>;
            dtos.Should().NotBeNull();
            dtos.Count.Should().Be(2);
            dtos.Select(d => d.Id).Should().BeEquivalentTo(alerts.Select(a => a.Id));
        }

        [Fact]
        public async Task GetTravelAlerts_ShouldReturnUnauthorized_WhenUserIdMissing()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetTravelAlerts();

            // Assert
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized.Should().NotBeNull();
            unauthorized.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetTravelAlert_ShouldReturnOk_WhenAlertExists()
        {
            // Arrange
            var alert = new TravelAlert
            {
                Id = "alert-1",
                Title = "Test Alert",
                Status = "Active",
                Timestamp = System.DateTime.UtcNow
            };

            _mockService.Setup(s => s.GetUserAlertByIdAsync(_userId, alert.Id))
                .ReturnsAsync(alert);

            // Act
            var result = await _controller.GetTravelAlert(alert.Id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);

            var dto = okResult.Value as TravelAlertDto;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(alert.Id);
            dto.Title.Should().Be(alert.Title);
        }

        [Fact]
        public async Task GetTravelAlert_ShouldReturnNotFound_WhenAlertDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetUserAlertByIdAsync(_userId, It.IsAny<string>()))
                .ReturnsAsync((TravelAlert)null);

            // Act
            var result = await _controller.GetTravelAlert("nonexistent-id");

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateTravelAlert_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateTravelAlertDto { Title = "New Alert", Status = "Active" };
            var createdAlert = new TravelAlert
            {
                Id = "new-id",
                Title = createDto.Title,
                Status = createDto.Status,
                Timestamp = System.DateTime.UtcNow
            };

            _mockService.Setup(s => s.CreateAlertAsync(_userId, createDto.Title, createDto.Status))
                .ReturnsAsync(createdAlert);

            // Act
            var result = await _controller.CreateTravelAlert(createDto);

            // Assert
            var createdAtAction = result as CreatedAtActionResult;
            createdAtAction.Should().NotBeNull();
            createdAtAction.StatusCode.Should().Be(201);
            createdAtAction.ActionName.Should().Be(nameof(_controller.GetTravelAlert));

            var dto = createdAtAction.Value as TravelAlertDto;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(createdAlert.Id);
        }

        [Fact]
        public async Task CreateTravelAlert_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var dto = new CreateTravelAlertDto();

            // Act
            var result = await _controller.CreateTravelAlert(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateTravelAlert_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var updateDto = new UpdateTravelAlertDto { Title = "Updated Title", Status = "Inactive" };
            _mockService.Setup(s => s.UpdateAlertAsync(_userId, "alert-1", updateDto.Title, updateDto.Status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTravelAlert("alert-1", updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateTravelAlert_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var dto = new UpdateTravelAlertDto();

            // Act
            var result = await _controller.UpdateTravelAlert("alert-1", dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateTravelAlert_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new UpdateTravelAlertDto { Title = "Title", Status = "Active" };
            _mockService.Setup(s => s.UpdateAlertAsync(_userId, "alert-1", updateDto.Title, updateDto.Status))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateTravelAlert("alert-1", updateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var statusDto = new UpdateAlertStatusDto { Status = "Resolved" };
            _mockService.Setup(s => s.UpdateAlertStatusAsync(_userId, "alert-1", statusDto.Status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateStatus("alert-1", statusDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            var statusDto = new UpdateAlertStatusDto { Status = "Resolved" };
            _mockService.Setup(s => s.UpdateAlertStatusAsync(_userId, "alert-1", statusDto.Status))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateStatus("alert-1", statusDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAlert_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAlertAsync(_userId, "alert-1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAlert("alert-1");

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteAlert_ShouldReturnNotFound_WhenDeleteFails()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAlertAsync(_userId, "alert-1"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAlert("alert-1");

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SeedTravelAlerts_ShouldReturnOk()
        {
            // Arrange
            _mockService.Setup(s => s.CreateSampleAlertsAsync(_userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SeedTravelAlerts();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task SeedTravelAlerts_ShouldReturnUnauthorized_WhenUserIdMissing()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.SeedTravelAlerts();

            // Assert
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized.Should().NotBeNull();
            unauthorized.StatusCode.Should().Be(401);
        }
    }
}
