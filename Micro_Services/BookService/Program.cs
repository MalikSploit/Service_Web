using Microsoft.EntityFrameworkCore;
using BookService.Data;


var builder = WebApplication.CreateBuilder(args);

// Database context setup
builder.Services.AddDbContext<BookServiceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BookServiceContext") ?? throw new InvalidOperationException("Connection string 'BookServiceContext' not found.")));


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
