using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public JobsController(ApplicationDbContext db) { _db = db; }

        // List all jobs
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Jobs.Include(j => j.Customer).Include(j => j.Product).Include(j => j.Dealer).Include(j => j.Technician).ToListAsync());

        // Get job by id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _db.Jobs.Include(j => j.Customer).Include(j => j.Product).Include(j => j.Dealer).Include(j => j.Technician).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // Create job (CCO/Dealer/Manager/Admin)
        [HttpPost]
        [Authorize(Roles = "CCO,Dealer,Manager,Admin")]
        public async Task<IActionResult> Create([FromBody] Job job)
        {
            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();
            // System-calculated claim: Only if technician assigned
            if (!string.IsNullOrEmpty(job.TechnicianId))
            {
                // Example calculation: 10% of job total (if invoice exists), else flat 500
                decimal amount = 500;
                var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.JobId == job.Id);
                if (invoice != null)
                    amount = Math.Max(500, invoice.Total * 0.10m);
                var claim = new TechnicianClaim
                {
                    JobId = job.Id,
                    TechnicianId = job.TechnicianId,
                    Amount = amount,
                    Status = TechnicianClaimStatus.Pending,
                    CreatedBy = User.Identity?.Name ?? "system",
                    CreatedAt = DateTime.UtcNow
                };
                _db.TechnicianClaims.Add(claim);
                await _db.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
        }

        // Assign technician (Manager/Admin/CCO)
        [HttpPost("{id}/assign-technician")]
        [Authorize(Roles = "Manager,Admin,CCO")]
        public async Task<IActionResult> AssignTechnician(int id, [FromBody] string technicianId)
        {
            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            job.TechnicianId = technicianId;
            await _db.SaveChangesAsync();
            return Ok(job);
        }

        // Update job status
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] JobStatus status)
        {
            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            job.Status = status;
            await _db.SaveChangesAsync();
            return Ok(job);
        }
    }
}
