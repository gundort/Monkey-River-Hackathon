using System.Security.Claims;
using BackEnd.DTOs.TravelAlertDTOs;
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
    public class TravelAlertsController : ControllerBase
    {
        private readonly ITravelAlertService _travelAlertService;

        public TravelAlertsController(ITravelAlertService travelAlertService)
        {
            _travelAlertService = travelAlertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTravelAlerts()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var alerts = await _travelAlertService.GetUserAlertsAsync(userId);

            var alertDtos = alerts.Select(a => new TravelAlertDto
            {
                Id = a.Id,
                Title = a.Title,
                Status = a.Status,
                Timestamp = a.Timestamp
            }).ToList();

            return Ok(alertDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTravelAlert(string id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var alert = await _travelAlertService.GetUserAlertByIdAsync(userId, id);

            if (alert == null)
                return NotFound(new { message = "Alert not found" });

            var alertDto = new TravelAlertDto
            {
                Id = alert.Id,
                Title = alert.Title,
                Status = alert.Status,
                Timestamp = alert.Timestamp
            };

            return Ok(alertDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTravelAlert([FromBody] CreateTravelAlertDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var alert = await _travelAlertService.CreateAlertAsync(userId, createDto.Title, createDto.Status);

            var alertDto = new TravelAlertDto
            {
                Id = alert.Id,
                Title = alert.Title,
                Status = alert.Status,
                Timestamp = alert.Timestamp
            };

            return CreatedAtAction(nameof(GetTravelAlert), new { id = alert.Id }, alertDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTravelAlert(string id, [FromBody] UpdateTravelAlertDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var success = await _travelAlertService.UpdateAlertAsync(userId, id, updateDto.Title, updateDto.Status);

            if (!success)
                return NotFound(new { message = "Alert not found or you don't have permission to update it" });

            return Ok(new { message = "Alert updated successfully" });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateAlertStatusDto statusDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var success = await _travelAlertService.UpdateAlertStatusAsync(userId, id, statusDto.Status);

            if (!success)
                return NotFound(new { message = "Alert not found or you don't have permission to update it" });

            return Ok(new { message = "Alert status updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(string id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var success = await _travelAlertService.DeleteAlertAsync(userId, id);

            if (!success)
                return NotFound(new { message = "Alert not found or you don't have permission to delete it" });

            return Ok(new { message = "Alert deleted successfully" });
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedTravelAlerts()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            await _travelAlertService.CreateSampleAlertsAsync(userId);

            return Ok(new { message = "Sample travel alerts created successfully" });
        }


        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
