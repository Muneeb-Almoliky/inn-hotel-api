using FastEndpoints;
using InnHotel.Infrastructure.Data;
using InnHotel.Web.Configurations;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Serilog.Extensions.Logging;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Database connection validation
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
ValidateDatabaseConnection(connectionString);

// Dependency Injection configuration
ConfigureServices(builder);

var app = builder.Build();

// Application startup configuration
await ConfigureApplication(app);

app.Run();

// Helper methods
static void ValidateDatabaseConnection(string? connectionString)
{
  if (string.IsNullOrEmpty(connectionString))
  {
    Log.Fatal("Database connection string is not configured");
    throw new InvalidOperationException("Connection string not found");
  }

  try
  {
    using var conn = new NpgsqlConnection(connectionString);
    conn.Open();
    Log.Information("Successfully connected to PostgreSQL database");
  }
  catch (PostgresException ex)
  {
    Log.Fatal("PostgreSQL Error {Code}: {Message}", ex.SqlState, ex.Message);
    throw;
  }
}

static void ConfigureServices(WebApplicationBuilder builder)
{
  var logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();

  builder.Services
      .AddOptionConfigs(builder.Configuration, logger, builder)
      .AddServiceConfigs(logger, builder)
      .AddFastEndpoints()
      .SwaggerDocument(o => o.ShortSchemaNames = true);
}

static async Task ConfigureApplication(WebApplication app)
{
  await app.UseAppMiddlewareAndSeedDatabase();
}

public partial class Program { }
