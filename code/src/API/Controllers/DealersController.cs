using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DealersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DealersController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Dealers.Include(d => d.Jobs).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dealer = await _db.Dealers.Include(d => d.Jobs).FirstOrDefaultAsync(d => d.Id == id);
            if (dealer == null) return NotFound();
            return Ok(dealer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dealer dealer)
        {
            _db.Dealers.Add(dealer);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dealer.Id }, dealer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Dealer dealer)
        {
            if (id != dealer.Id) return BadRequest();
            _db.Entry(dealer).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dealer = await _db.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();
            _db.Dealers.Remove(dealer);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
