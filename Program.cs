/*
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
 
Application                    : Car Rental Management System
Trainee Name                   : Srivartshan
Reviewer                       :
Date Created                   : 02/12/2024
Modified Date                  : 05/01/2025
Purpose of the Programme       : To manage car details
Language used                  : .NET Core C#
Framework used                 : ASP NET Core MVC
Database                       : SQL Server
Reviewed Date                  :
 
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
*/

using System.Text;
using CarRentalManagement.Controllers;
using CarRentalManagement.Data;
using CarRentalManagement.Models;
using CarRentalManagement.Repositories;
using IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Console logging (plain text)
    .WriteTo.File(
        path: "logs/log-.json",  // Use `.json` extension for clarity
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        formatter: new Serilog.Formatting.Json.JsonFormatter() // Enable JSON formatting
    )
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add HTTP client services
builder.Services.AddHttpClient();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Error($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information($"Token validated for user: {context.Principal.Identity.Name}");
            return Task.CompletedTask;
        }
    };
});

// Add DbContext to the service container
builder.Services.AddDbContext<CarRentalContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }
});

// Add Session Support
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".CarRentalApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add MVC services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IGenericRepository<Car>,GenericRepository<Car>>();
builder.Services.AddTransient<IGenericRepository<Admin>,GenericRepository<Admin>>();
builder.Services.AddScoped<IGenericRepository<Booking>,GenericRepository<Booking>>();
builder.Services.AddScoped<IApiRepository<Customer>>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var apiUrl = configuration["ApiUrls:Customer"];
    return new ApiRepository<Customer>(
        provider.GetRequiredService<IHttpClientFactory>(),
        apiUrl
    );
});




var app = builder.Build();

// Global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception exception)
    {
        Log.Error($"Unhandled exception: {exception}");
        throw;
    }
});

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseRouting();

// Enable session handling
app.UseSession();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
