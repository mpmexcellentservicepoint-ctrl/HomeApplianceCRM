using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public InvoicesController(ApplicationDbContext db) { _db = db; }

        // GET: api/invoices/job/{jobId}
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var invoice = await _db.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.JobId == jobId);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        // POST: api/invoices/generate/{jobId}
        [HttpPost("generate/{jobId}")]
        public async Task<IActionResult> Generate(int jobId)
        {
            var job = await _db.Jobs.Include(j => j.JobParts).Include(j => j.ServiceCharges).FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null) return NotFound();
            // Calculate charges
            decimal partsCharge = 0;
            foreach (var jp in job.JobParts) partsCharge += jp.Quantity * (jp.Part?.Price ?? 0);
            decimal serviceCharge = job.ServiceCharges.Sum(sc => sc.Amount);
            decimal gst = 0.18m * (partsCharge + serviceCharge);
            decimal total = partsCharge + serviceCharge + gst;
            var invoice = new Invoice
            {
                JobId = jobId,
                Total = total,
                InvoiceDate = DateTime.UtcNow,
                Payments = new List<Payment>()
            };
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return Ok(invoice);
        }
        // POST: api/invoices/{invoiceId}/send-log
        [HttpPost("{invoiceId}/send-log")]
        public async Task<IActionResult> LogSendAttempt(int invoiceId, [FromBody] LogSendAttemptDto dto)
        {
            var invoice = await _db.Invoices.FindAsync(invoiceId);
            if (invoice == null) return NotFound();
            var log = new InvoiceSendLog
            {
                InvoiceId = invoiceId,
                Method = dto.Method,
                Recipient = dto.Recipient,
                SentAt = DateTime.UtcNow,
                Status = dto.Status,
                Details = dto.Details,
                CreatedBy = User.Identity?.Name ?? "system",
                CreatedAt = DateTime.UtcNow
            };
            _db.InvoiceSendLogs.Add(log);
            await _db.SaveChangesAsync();
            return Ok(log);
        }

        // GET: api/invoices/{invoiceId}/send-logs
        [HttpGet("{invoiceId}/send-logs")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetSendLogs(int invoiceId)
        {
            var logs = await _db.InvoiceSendLogs
                .Where(l => l.InvoiceId == invoiceId)
                .OrderByDescending(l => l.SentAt)
                .ToListAsync();
            return Ok(logs);
        }
    }

    // DTO for logging send attempts
    public class LogSendAttemptDto
    {
        public string Method { get; set; } = string.Empty; // Email, WhatsApp
        public string Recipient { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Details { get; set; }
    }
}
