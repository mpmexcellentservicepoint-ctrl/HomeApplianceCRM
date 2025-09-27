using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PartsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/parts
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var parts = await _db.Parts.ToListAsync();
            return Ok(parts);
        }

        // GET: api/parts/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var part = await _db.Parts.FindAsync(id);
            if (part == null) return NotFound();
            return Ok(part);
        }

        // POST: api/parts
        [HttpPost]
        [Authorize(Roles = "Storekeeper,Admin")]
        public async Task<IActionResult> Add([FromBody] Part part)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.Parts.Add(part);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = part.Id }, part);
        }

        // PUT: api/parts/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Storekeeper,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Part part)
        {
            if (id != part.Id) return BadRequest();
            _db.Entry(part).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/parts/assign
        [HttpPost("assign")]
        [Authorize]
        public async Task<IActionResult> AssignToJob([FromBody] JobPart jobPart)
        {
            var part = await _db.Parts.FindAsync(jobPart.PartId);
            if (part == null) return NotFound("Part not found");
            if (part.StockQty < jobPart.Quantity) return BadRequest("Insufficient stock");
            part.StockQty -= jobPart.Quantity;
            _db.JobParts.Add(jobPart);
            await _db.SaveChangesAsync();
            // Check MSL
            if (part.StockQty < part.MSL)
            {
                // Optionally: log, notify, or mark shortage
            }
            return Ok(jobPart);
        }

        // POST: api/parts/request
        [HttpPost("request")]
        [Authorize]
        public async Task<IActionResult> RequestPart([FromBody] PartRequest req)
        {
            var part = await _db.Parts.FindAsync(req.PartId);
            if (part == null) return NotFound("Part not found");
            req.Status = "Requested";
            _db.PartRequests.Add(req);
            await _db.SaveChangesAsync();
            return Ok(req);
        }

        // POST: api/parts/return
        [HttpPost("return")]
        [Authorize]
        public async Task<IActionResult> ReturnPart([FromBody] JobPart jobPart)
        {
            var part = await _db.Parts.FindAsync(jobPart.PartId);
            if (part == null) return NotFound("Part not found");
            part.StockQty += jobPart.Quantity;
            _db.JobParts.Remove(jobPart);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
