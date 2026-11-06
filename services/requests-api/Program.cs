using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RequestsApi.Data;
using RequestsApi.Clients;
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

#region Builder Services

// Registers the DbContext (scoped: one per HTTP request) and connects it to SQL Server
builder.Services.AddDbContext<RequestsDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("RequestsDb"),
        // Retries the connection if SQL Server isn't ready yet (important in Docker)
        sql => sql.EnableRetryOnFailure()));

// Turns on controllers, and sends enums as text ("Submitted") instead of numbers (0)
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Lets Swagger discover the endpoints
builder.Services.AddEndpointsApiExplorer();

// Generates the Swagger documentation for the API
builder.Services.AddSwaggerGen();

// Adds a health check that also confirms the database is reachable
builder.Services.AddHealthChecks().AddDbContextCheck<RequestsDbContext>();

// 3 s per attempt, up to 2 retries, 10 s total
Action<HttpStandardResilienceOptions> resilience = o =>
{
    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
    o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
    o.Retry.MaxRetryAttempts = 2;
};

builder.Services.AddHttpClient<CountriesClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:CountriesApi"]!))
    .AddStandardResilienceHandler(resilience);

builder.Services.AddHttpClient<EligibilityClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:EligibilityApi"]!))
    .AddStandardResilienceHandler(resilience);

// Which websites may call this API from a browser (React locally, and later Static Web Apps)
var allowedOrigins = builder.Configuration["AllowedOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));
#endregion

var app = builder.Build();

// Applies any pending migrations at startup, so the database creates itself
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<RequestsDbContext>().Database.Migrate();

// Serves the Swagger JSON and the Swagger UI page (on in every environment, so it works in Docker and Azure)
app.UseSwagger();
app.UseSwaggerUI();

// Adds the CORS headers so the browser lets React read the responses
app.UseCors("Frontend");

// Connects the controller routes, like /api/requests
app.MapControllers();

// Exposes /health, which Docker and Azure use to check the service is alive
app.MapHealthChecks("/health");

app.Run();