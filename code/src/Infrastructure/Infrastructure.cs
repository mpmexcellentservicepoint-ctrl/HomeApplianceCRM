namespace Infrastructure {
    // Infrastructure services, EF Core DbContext, and repository implementations will be defined here.

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public enum TechnicianClaimStatus
    {
        Pending,
        Approved,
        Rejected,
        Hold
    }

    public class TechnicianClaim : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public string TechnicianId { get; set; } = string.Empty;
        [ForeignKey("TechnicianId")] public virtual AppUser Technician { get; set; } = null!;
        public decimal Amount { get; set; }
        public TechnicianClaimStatus Status { get; set; } = TechnicianClaimStatus.Pending;
        public string? Comment { get; set; }
        public DateTime? DecisionDate { get; set; }
    }

    public class InvoiceSendLog : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")] public virtual Invoice Invoice { get; set; } = null!;
        public string Method { get; set; } = string.Empty; // Email, WhatsApp
        public string Recipient { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string? Status { get; set; }
        public string? Details { get; set; }
    }


    public abstract class AuditableEntity
    {
        [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AppUser : IdentityUser
    {
        public string Role { get; set; } = string.Empty;
        public virtual Customer? Customer { get; set; }
        public virtual Dealer? Dealer { get; set; }
    }

    public class Customer : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(100)] public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")] public virtual AppUser? User { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }

    public class Dealer : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(100)] public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")] public virtual AppUser? User { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }

    public enum WarrantyType
    {
        Standard,
        Promotional,
        OutOfWarranty
    }

    public class Product : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ModelNumber { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")] public virtual Customer? Customer { get; set; }
        public int? DealerId { get; set; }
        [ForeignKey("DealerId")] public virtual Dealer? Dealer { get; set; }
        public WarrantyType WarrantyType { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }

    public enum JobStatus
    {
        Assigned,
        Allocate,
        PartPending,
        CustomerChangeAppointmentDate,
        WorkCompleted,
        Closed,
        Cancelled
    }

    public enum SLAStatus
    {
        OnTrack,
        Nearing,
        Breached,
        Escalated
    }

    public class Job : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")] public virtual Customer? Customer { get; set; }
        public int? DealerId { get; set; }
        [ForeignKey("DealerId")] public virtual Dealer? Dealer { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")] public virtual Product? Product { get; set; }
        public string? TechnicianId { get; set; }
        [ForeignKey("TechnicianId")] public virtual AppUser? Technician { get; set; }
        public JobStatus Status { get; set; }
        public int SLAHours { get; set; }
        public DateTime? SLADeadline { get; set; }
        public SLAStatus SLAStatus { get; set; }
        public virtual ICollection<JobPart> JobParts { get; set; } = new List<JobPart>();
        public virtual ICollection<ServiceCharge> ServiceCharges { get; set; } = new List<ServiceCharge>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class Part : AuditableEntity
    {
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; } // For invoice calculation
    public int StockQty { get; set; }
    public int MSL { get; set; } // Minimum Stock Level
    [MaxLength(100)] public string? StorageLocation { get; set; }
    [MaxLength(50)] public string? BoxNo { get; set; }
    [MaxLength(100)] public string? AlternateLocation { get; set; }
    public virtual ICollection<JobPart> JobParts { get; set; } = new List<JobPart>();
    public virtual ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
    public bool IsBelowMSL() => StockQty < MSL;
    }

    public class JobPart : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public int PartId { get; set; }
        [ForeignKey("PartId")] public virtual Part Part { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class PartRequest : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int PartId { get; set; }
        [ForeignKey("PartId")] public virtual Part Part { get; set; } = null!;
        public int? JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job? Job { get; set; }
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")] public virtual Supplier? Supplier { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }
    }

    public class Supplier : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public virtual ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    public class PurchaseOrder : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")] public virtual Supplier Supplier { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
    }

    public class ServiceCharge : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class Invoice : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public decimal Total { get; set; }
        public DateTime InvoiceDate { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class Payment : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")] public virtual Invoice Invoice { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Method { get; set; }
    }

    public class Claim : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public string? Description { get; set; }
        public string? Status { get; set; }
    }

    public class Notification : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")] public virtual Job Job { get; set; } = null!;
        public string? Message { get; set; }
        public bool IsRead { get; set; }
    }

    public class TechnicianLocation : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public string TechnicianId { get; set; } = string.Empty;
        [ForeignKey("TechnicianId")] public virtual AppUser Technician { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuditLog : AuditableEntity
    {
        [Key] public int Id { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? Details { get; set; }
    }

    public class KBArticle : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }

    public class Setting : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class RefreshToken : AuditableEntity
    {
        [Key] public int Id { get; set; }
        [Required] public string Token { get; set; } = string.Empty;
        [Required] public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")] public virtual AppUser User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Dealer> Dealers => Set<Dealer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<JobPart> JobParts => Set<JobPart>();
        public DbSet<PartRequest> PartRequests => Set<PartRequest>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<ServiceCharge> ServiceCharges => Set<ServiceCharge>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TechnicianLocation> TechnicianLocations => Set<TechnicianLocation>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<KBArticle> KBArticles => Set<KBArticle>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<InvoiceSendLog> InvoiceSendLogs => Set<InvoiceSendLog>();
        public DbSet<TechnicianClaim> TechnicianClaims => Set<TechnicianClaim>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes and relationships
            modelBuilder.Entity<Customer>().HasIndex(x => x.Email);
            modelBuilder.Entity<Dealer>().HasIndex(x => x.Email);
            modelBuilder.Entity<Product>().HasIndex(x => x.Name);
            modelBuilder.Entity<Part>().HasIndex(x => x.Name);
            modelBuilder.Entity<Job>().HasIndex(x => x.Status);
            modelBuilder.Entity<PartRequest>().HasIndex(x => x.Status);
            modelBuilder.Entity<PurchaseOrder>().HasIndex(x => x.OrderDate);
            modelBuilder.Entity<Invoice>().HasIndex(x => x.InvoiceDate);
            modelBuilder.Entity<Payment>().HasIndex(x => x.PaymentDate);
            modelBuilder.Entity<KBArticle>().HasIndex(x => x.Title);
            modelBuilder.Entity<Setting>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<RefreshToken>().HasIndex(x => x.Token).IsUnique();
        }
    }
}
