using CriticalThinkingGame.ApiService.Data;
using CriticalThinkingGame.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add controllers
builder.Services.AddControllers();

// Add Entity Framework
builder.AddNpgsqlDbContext<GameDbContext>("criticalthinkinggame");

// Add custom services
builder.Services.AddScoped<IGameService, GameService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use CORS
app.UseCors();

// Map controllers
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => "Healthy");

app.MapDefaultEndpoints();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DataSeeder.SeedDataAsync(context);
}

await app.RunAsync();
