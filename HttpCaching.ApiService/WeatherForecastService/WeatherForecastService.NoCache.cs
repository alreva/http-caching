using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<Ok<WeatherForecast[]>> NoCache()
    {
        await Task.Delay(weatherOptions.Wait);
        return TypedResults.Ok(Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray());
    }
}