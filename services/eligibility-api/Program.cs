using Microsoft.EntityFrameworkCore;
using EligibilityApi.Data;
using EligibilityApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Database: connection string comes from user secrets locally
builder.Services.AddDbContext<EligibilityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("EligibilityDb"),
        sql => sql.EnableRetryOnFailure()));   // retries brief SQL outages (useful on Azure)

// Our service: one instance per HTTP request, same lifetime as the DbContext
builder.Services.AddScoped<IEligibilityService, EligibilityService>();

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health check that also confirms the database is reachable
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EligibilityDbContext>();

var app = builder.Build();

// Apply migrations at startup (creates the table and seed rows if missing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EligibilityDbContext>();
    db.Database.Migrate();
}

// Swagger always on, not just in Development, so it works in Docker/Azure too
app.UseSwagger();
app.UseSwaggerUI();

// No HTTPS redirect: containers and Azure handle HTTPS outside the app

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();