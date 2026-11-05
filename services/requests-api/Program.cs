using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RequestsApi.Data;
using RequestsApi.Clients;

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

builder.Services.AddHttpClient<EligibilityClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:EligibilityApi"]!));

#endregion

var app = builder.Build();

// Applies any pending migrations at startup, so the database creates itself
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<RequestsDbContext>().Database.Migrate();

// Serves the Swagger JSON and the Swagger UI page (on in every environment, so it works in Docker and Azure)
app.UseSwagger();
app.UseSwaggerUI();

// Connects the controller routes, like /api/requests
app.MapControllers();

// Exposes /health, which Docker and Azure use to check the service is alive
app.MapHealthChecks("/health");

app.Run();