
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();


// Serilog configuration
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(ctx.Configuration));

// EF Core and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=HomeApplianceCRM;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "THIS IS A VERY SECURE KEY";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HomeApplianceCRM";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Authorization policies for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RequireCCORole", policy => policy.RequireRole("CCO"));
    options.AddPolicy("RequireTechnicianRole", policy => policy.RequireRole("Technician"));
    options.AddPolicy("RequireStorekeeperRole", policy => policy.RequireRole("Storekeeper"));
    options.AddPolicy("RequireDealerRole", policy => policy.RequireRole("Dealer"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
});

// Enforce 2FA for Admins
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


// Placeholder for API endpoints
// Add your controllers/endpoints here




// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    string[] roles = new[] { "Admin", "Manager", "CCO", "Technician", "Storekeeper", "Dealer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed one user per role
    var users = new[] {
        new { Email = "admin@crm.com", Password = "Admin123!", Role = "Admin" },
        new { Email = "manager@crm.com", Password = "Manager123!", Role = "Manager" },
        new { Email = "cco@crm.com", Password = "CCO123!", Role = "CCO" },
        new { Email = "tech@crm.com", Password = "Tech123!", Role = "Technician" },
        new { Email = "store@crm.com", Password = "Store123!", Role = "Storekeeper" },
        new { Email = "dealer@crm.com", Password = "Dealer123!", Role = "Dealer" }
    };
    foreach (var u in users)
    {
        var user = await userManager.FindByEmailAsync(u.Email);
        if (user == null)
        {
            user = new AppUser { UserName = u.Email, Email = u.Email, Role = u.Role };
            await userManager.CreateAsync(user, u.Password);
            await userManager.AddToRoleAsync(user, u.Role);
        }
    }
}

app.MapControllers();
app.Run();


// End of file
