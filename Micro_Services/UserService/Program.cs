using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using Entities;

var builder = WebApplication.CreateBuilder(args);

// Database context setup
builder.Services.AddDbContext<UserServiceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UserServiceContext") ?? throw new InvalidOperationException("Connection string 'UserServiceContext' not found.")));

// Add services to the container
builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and create database at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<UserServiceContext>();
    dbContext.Database.Migrate(); // This line will apply pending migrations
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();