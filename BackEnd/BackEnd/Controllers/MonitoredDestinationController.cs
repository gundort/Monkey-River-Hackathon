using BackEnd.DTOs;
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
    public class MonitoredDestinationController : ControllerBase
    {
        private readonly IEntityService<MonitoredDestination> _service;

        public MonitoredDestinationController(IEntityService<MonitoredDestination> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var destination = await _service.GetByIdAsync(id);
            return destination == null ? NotFound() : Ok(destination);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MonitoredDestinationDto dto)
        {
            var newDest = new MonitoredDestination
            {
                Location = dto.Location,
                RiskLevel = dto.RiskLevel,
                LastChecked = dto.LastChecked
            };

            await _service.CreateAsync(newDest);
            return CreatedAtAction(nameof(GetById), new { id = newDest.Id }, newDest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, MonitoredDestinationDto dto)
        {
            var updated = new MonitoredDestination
            {
                Id = id,
                Location = dto.Location,
                RiskLevel = dto.RiskLevel,
                LastChecked = dto.LastChecked
            };

            await _service.UpdateAsync(id, updated);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
