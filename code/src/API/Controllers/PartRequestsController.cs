using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PartRequestsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/partrequests
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _db.PartRequests.Include(r => r.Part).Include(r => r.Job).ToListAsync();
            return Ok(requests);
        }

        // POST: api/partrequests/request
        [HttpPost("request")]
        [Authorize]
        public async Task<IActionResult> RequestPart([FromBody] PartRequest req)
        {
            var part = await _db.Parts.FindAsync(req.PartId);
            if (part == null) return NotFound("Part not found");
            if (part.StockQty >= req.Quantity)
            {
                part.StockQty -= req.Quantity;
                req.Status = "Issued";
            }
            else
            {
                req.Status = "Pending";
            }
            _db.PartRequests.Add(req);
            await _db.SaveChangesAsync();
            return Ok(req);
        }

        // POST: api/partrequests/arrived/{id}
        [HttpPost("arrived/{id}")]
        [Authorize(Roles = "Storekeeper,Admin")]
        public async Task<IActionResult> MarkArrived(int id)
        {
            var req = await _db.PartRequests.Include(r => r.Part).Include(r => r.Job).FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound();
            req.Status = "Arrived";
            if (req.Part != null)
                req.Part.StockQty += req.Quantity;
            await _db.SaveChangesAsync();
            // TODO: Notification logic here
            // If all parts for job arrived, re-allocate job
            if (req.JobId.HasValue)
            {
                var jobPartsPending = await _db.PartRequests.Where(r => r.JobId == req.JobId && r.Status != "Arrived").AnyAsync();
                if (!jobPartsPending)
                {
                    var job = await _db.Jobs.FindAsync(req.JobId);
                    if (job != null)
                    {
                        job.Status = JobStatus.Allocate;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return Ok(req);
        }
    }
}
