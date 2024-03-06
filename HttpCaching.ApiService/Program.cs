using HttpCaching.ApiService.WeatherForecastService;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(configure =>
{
    configure.Path = "appsettings.live.json";
    configure.ReloadOnChange = true;
});

builder.Services.AddTransient<WeatherOptions>(provider =>
{
    WeatherOptions config = new();
    provider.GetRequiredService<IConfiguration>().GetSection("Weather").Bind(config);
    return config;
});

builder.Services.AddTransient<WeatherForecastService>();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy =>
    policy
        .WithOrigins("http://localhost:5213")
        .AllowAnyHeader()
        .WithMethods("GET"));

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet(
    "/weatherforecast",
    Task<Ok<WeatherForecast[]>> (WeatherForecastService svc) => svc.NoCache());

app.MapGet(
    "/weatherforecast-lastmodified",
    Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > (WeatherForecastService svc, HttpContext ctx) => svc.LastModified(ctx));

app.MapDefaultEndpoints();

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class WeatherOptions
{
    public TimeSpan Wait { get; set; }
}
