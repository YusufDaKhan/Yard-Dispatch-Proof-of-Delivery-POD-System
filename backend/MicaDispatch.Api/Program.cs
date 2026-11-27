using MicaDispatch.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Prevents circular reference issues during JSON serialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Converts enums to their string representation in JSON
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDevPolicy", builder =>
    {
        builder.AllowAnyMethod()
               .AllowAnyHeader()
               .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:8081");
    });
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseCors("FrontendDevPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
