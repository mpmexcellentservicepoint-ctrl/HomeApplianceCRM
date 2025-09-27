using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPartsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public JobPartsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/jobparts/job/{jobId}
        [HttpGet("job/{jobId}")]
        [Authorize]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var jobParts = await _db.JobParts.Include(jp => jp.Part).Where(jp => jp.JobId == jobId).ToListAsync();
            return Ok(jobParts);
        }

        // POST: api/jobparts/markused
        [HttpPost("markused")]
        [Authorize]
        public async Task<IActionResult> MarkUsed([FromBody] JobPart jobPart)
        {
            var jp = await _db.JobParts.FindAsync(jobPart.Id);
            if (jp == null) return NotFound();
            // Mark as used (could add a Used flag or status if needed)
            // For now, just acknowledge
            return Ok(jp);
        }

        // POST: api/jobparts/return
        [HttpPost("return")]
        [Authorize]
        public async Task<IActionResult> ReturnUnused([FromBody] JobPart jobPart)
        {
            var jp = await _db.JobParts.FindAsync(jobPart.Id);
            if (jp == null) return NotFound();
            var part = await _db.Parts.FindAsync(jp.PartId);
            if (part != null) part.StockQty += jp.Quantity;
            _db.JobParts.Remove(jp);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
