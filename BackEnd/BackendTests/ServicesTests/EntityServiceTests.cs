using BackEnd.Repositories.Interfaces;
using BackEnd.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests.ServicesTests
{
    public class EntityServiceTests
    {
        private readonly Mock<IRepository<DummyEntity>> _mockRepository;
        private readonly EntityService<DummyEntity> _service;

        public EntityServiceTests()
        {
            _mockRepository = new Mock<IRepository<DummyEntity>>();
            _service = new EntityService<DummyEntity>(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallRepositoryGetAll()
        {
            // Arrange
            var entities = new List<DummyEntity> { new DummyEntity() };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(entities, result);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldCallRepositoryGetById()
        {
            var id = "123";
            var entity = new DummyEntity();
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.Equal(entity, result);
            _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallRepositoryAddAsync()
        {
            var entity = new DummyEntity();

            await _service.CreateAsync(entity);

            _mockRepository.Verify(r => r.AddAsync(entity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdateAsync()
        {
            var id = "456";
            var entity = new DummyEntity();

            await _service.UpdateAsync(id, entity);

            _mockRepository.Verify(r => r.UpdateAsync(id, entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteAsync()
        {
            var id = "789";

            await _service.DeleteAsync(id);

            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        public class DummyEntity { }
    }

}
