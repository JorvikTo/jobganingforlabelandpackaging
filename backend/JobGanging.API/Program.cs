using JobGanging.Core.Interfaces;
using JobGanging.Core.Services;
using JobGanging.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for Vue.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register application services
builder.Services.AddScoped<IDieLineService, DieLineService>();
builder.Services.AddScoped<INestingService, NestingService>();
builder.Services.AddScoped<ISheetService, SheetService>();
builder.Services.AddScoped<IExportService, ExportService>();

// Configure database (using In-Memory for demo)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("JobGangingDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("VueApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
