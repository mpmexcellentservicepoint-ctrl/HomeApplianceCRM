using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CustomersController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Customers.Include(c => c.Jobs).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _db.Customers.Include(c => c.Jobs).FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id) return BadRequest();
            _db.Entry(customer).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
