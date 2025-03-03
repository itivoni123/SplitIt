using SplititActorsApi.Data;
using SplititActorsApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ActorsDB"));

// Register the ActorScraperService for DI
builder.Services.AddScoped<ActorScraperService>();

// Register HttpClient and AuthService
builder.Services.AddHttpClient<AuthService>(); // Registers HttpClient to be used by AuthService
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth")); // Configure AuthConfig from appsettings.json

// Add controllers to the service container
builder.Services.AddControllers();

// Add Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
