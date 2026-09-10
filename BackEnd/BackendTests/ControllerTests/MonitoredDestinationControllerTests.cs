using BackEnd.Controllers;
using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.ControllerTests
{
    public class MonitoredDestinationControllerTests
    {
        private readonly Mock<IEntityService<MonitoredDestination>> _mockService;
        private readonly MonitoredDestinationController _controller;

        public MonitoredDestinationControllerTests()
        {
            _mockService = new Mock<IEntityService<MonitoredDestination>>();
            _controller = new MonitoredDestinationController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithList()
        {
            // Arrange
            var destinations = new List<MonitoredDestination>
            {
                new MonitoredDestination { Id = "1", Location = "City A", RiskLevel = "Low", LastChecked = DateTime.UtcNow },
                new MonitoredDestination { Id = "2", Location = "City B", RiskLevel = "High", LastChecked = DateTime.UtcNow }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(destinations);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(destinations);
        }

        [Fact]
        public async Task GetById_ShouldReturnDestination_WhenFound()
        {
            // Arrange
            var id = "1";
            var dest = new MonitoredDestination
            {
                Id = id,
                Location = "City A",
                RiskLevel = "Low",
                LastChecked = DateTime.UtcNow
            };

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dest);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(dest);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var id = "missing";
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((MonitoredDestination)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var dto = new MonitoredDestinationDto
            {
                Location = "City A",
                RiskLevel = "Moderate",
                LastChecked = DateTime.UtcNow
            };

            MonitoredDestination captured = null!;
            _mockService.Setup(s => s.CreateAsync(It.IsAny<MonitoredDestination>()))
                .Callback<MonitoredDestination>(m => { m.Id = "new-id"; captured = m; })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.RouteValues["id"].Should().Be("new-id");
            createdResult.Value.Should().BeEquivalentTo(captured);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            // Arrange
            var id = "update-id";
            var dto = new MonitoredDestinationDto
            {
                Location = "Updated City",
                RiskLevel = "High",
                LastChecked = DateTime.UtcNow
            };

            _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<MonitoredDestination>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var id = "delete-id";
            _mockService.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
