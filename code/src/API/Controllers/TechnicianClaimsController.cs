using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TechnicianClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TechnicianClaimsController(ApplicationDbContext db) { _db = db; }

        // GET: api/technicianclaims/queue
        [HttpGet("queue")]
        [Authorize(Roles = "Admin,Manager,CCO")]
        public async Task<IActionResult> GetApprovalQueue()
        {
            var claims = await _db.TechnicianClaims
                .Include(tc => tc.Job)
                .Include(tc => tc.Technician)
                .Where(tc => tc.Status == TechnicianClaimStatus.Pending || tc.Status == TechnicianClaimStatus.Hold)
                .ToListAsync();
            return Ok(claims);
        }

        // POST: api/technicianclaims/{id}/decision
        [HttpPost("{id}/decision")]
        [Authorize(Roles = "Admin,Manager,CCO")]
        public async Task<IActionResult> Decide(int id, [FromBody] ClaimDecisionDto dto)
        {
            var claim = await _db.TechnicianClaims.FindAsync(id);
            if (claim == null) return NotFound();
            if (claim.Status != TechnicianClaimStatus.Pending && claim.Status != TechnicianClaimStatus.Hold)
                return BadRequest("Claim already decided.");
            claim.Status = dto.Status;
            claim.Comment = dto.Comment;
            claim.DecisionDate = DateTime.UtcNow;
            claim.UpdatedBy = User.Identity?.Name ?? "system";
            claim.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(claim);
        }

        // GET: api/technicianclaims/my
        [HttpGet("my")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var claims = await _db.TechnicianClaims
                .Include(tc => tc.Job)
                .Where(tc => tc.TechnicianId == userId && tc.Status == TechnicianClaimStatus.Approved)
                .ToListAsync();
            return Ok(claims);
        }

        // GET: api/technicianclaims/job/{jobId}
        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Admin,Manager,CCO,Technician")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var claim = await _db.TechnicianClaims.Include(tc => tc.Technician).FirstOrDefaultAsync(tc => tc.JobId == jobId);
            if (claim == null) return NotFound();
            // Only allow technician to see if approved
            if (User.IsInRole("Technician") && claim.Status != TechnicianClaimStatus.Approved)
                return Forbid();
            return Ok(claim);
        }
    }

    public class ClaimDecisionDto
    {
        public TechnicianClaimStatus Status { get; set; }
        public string? Comment { get; set; }
    }
}
