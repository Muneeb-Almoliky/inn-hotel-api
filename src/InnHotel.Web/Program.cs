﻿using System.Text;
using InnHotel.Core.AuthAggregate;
using InnHotel.Infrastructure.Data;
using InnHotel.Web.Common;
using InnHotel.Web.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("AdminsOnly", policy =>
   policy.RequireRole(Roles.Admin, Roles.SuperAdmin));
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var error = new FailureResponse(
                401,
                "Unauthorized: authentication is required."
            );

            return context.Response.WriteAsJsonAsync(error);
        }
    };
});
builder.Services.AddAuthentication(); 
builder.Services.AddAuthorization();


// Dependency Injection configuration
ConfigureServices(builder);

var app = builder.Build();

app.UsePathBase("/api");

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
      .SwaggerDocument(o => {
        o.ShortSchemaNames = true;
        o.AutoTagPathSegmentIndex = 2;
        });
}

static async Task ConfigureApplication(WebApplication app)
{
  await app.UseAppMiddlewareAndSeedDatabase();
}

public partial class Program { }
