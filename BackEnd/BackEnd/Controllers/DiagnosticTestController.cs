using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DiagnosticTestController : ControllerBase
    {
        private readonly IEntityService<DiagnosticTest> _service;

        public DiagnosticTestController(IEntityService<DiagnosticTest> service)
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
            var test = await _service.GetByIdAsync(id);
            return test == null ? NotFound() : Ok(test);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiagnosticTestDto dto)
        {
            var newTest = new DiagnosticTest
            {
                Name = dto.Name,
                Result = dto.Result,
                Date = dto.Date
            };

            await _service.CreateAsync(newTest);
            return CreatedAtAction(nameof(GetById), new { id = newTest.Id }, newTest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, DiagnosticTestDto dto)
        {
            var updated = new DiagnosticTest
            {
                Id = id,
                Name = dto.Name,
                Result = dto.Result,
                Date = dto.Date
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
