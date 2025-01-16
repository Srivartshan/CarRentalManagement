using CarRentalManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers support for Web API
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow all origins
              .AllowAnyMethod()  // Allow all HTTP methods (GET, POST, etc.)
              .AllowAnyHeader(); // Allow all headers
    });
});

// Enable Swagger for API documentation (Use this for documenting API endpoints)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in Development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Enable authorization (if needed for your API)
app.UseAuthorization();

// Map controllers (this automatically maps routes to API controllers)
app.MapControllers();

app.Run();
