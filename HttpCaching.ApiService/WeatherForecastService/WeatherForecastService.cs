using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

public partial class WeatherForecastService(WeatherOptions weatherOptions)
{
    private StatusCodeHttpResult EmptyResultWith304NotModifiedStatus(HttpContext ctx)
    {
        ctx.Response.StatusCode = StatusCodes.Status304NotModified;
        ctx.Response.Headers.LastModified = weatherOptions.LastModified.ToString("R");
        ctx.Response.Headers.ETag = weatherOptions.ETag;
        return TypedResults.StatusCode(StatusCodes.Status304NotModified);
    }
}