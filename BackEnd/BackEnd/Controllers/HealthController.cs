using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly MongoDbContext _db;
        public HealthController(MongoDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Check()
        {
            try
            {
                _db.DiagnosticTests.Find(_ => true).Limit(1).ToList();
                return Ok("MongoDB connection is healthy.");
            }
            catch
            {
                return StatusCode(500, "MongoDB connection failed.");
            }
        }
    }
}
