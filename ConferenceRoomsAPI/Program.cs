using ConferenceRoomsAPI.ExeptionHandler;
using ConferenceRoomsAPI.Infrastructure.Database;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using ConferenceRoomsAPI.Infrastructure.Repositories;
using ConferenceRoomsAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

// Infrastructure Services
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<DataSeeder>();

// Application Services
builder.Services.AddScoped<RoomAppService>();
builder.Services.AddScoped<ServiceAppService>();
builder.Services.AddScoped<BookingAppService>();
builder.Services.AddScoped<ReportService>();

// Controllers + JSON 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Conference Rooms API",
        Version = "v1",
        Description = "API for managing conference room bookings"
    });
});

// Global Exception Handler 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Seeding 
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Middleware Pipeline 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Rooms API v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
