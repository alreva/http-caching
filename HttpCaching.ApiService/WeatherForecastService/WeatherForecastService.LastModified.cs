using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    private static readonly ConcurrentDictionary<string, DateTimeOffset> LastModifiedRepo = new();
    
    public async Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > LastModified(HttpContext ctx)
    {
        if (!TryGetModifiedSinceFromHeaders(ctx.Request.Headers, out var lastModified))
        {
            return await NoCacheWithLastModifiedResponseHeader(ctx);
        }

        if (ModifiedLaterThanRequested(lastModified))
        {
            return await NoCacheWithLastModifiedResponseHeader(ctx);
        }

        return EmptyResultWith304NotModifiedStatus(ctx);
    }

    #region Private methods

    private static bool TryGetModifiedSinceFromHeaders(
        IHeaderDictionary requestHeaders,
        out DateTimeOffset lastModified)
    {
        return DateTimeOffset.TryParseExact(
            requestHeaders.IfModifiedSince,
            "R", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
            out lastModified);
    }

    private static bool ModifiedLaterThanRequested(DateTimeOffset lastModified)
    {
        if (!LastModifiedRepo.ContainsKey("weather-forecast"))
        {
            return false;
        }

        return LastModifiedRepo["weather-forecast"] > lastModified;
    }

    private Task<Ok<WeatherForecast[]>> NoCacheWithLastModifiedResponseHeader(HttpContext ctx)
    {
        var now = DateTimeOffset.UtcNow;
        var newLastModified = new DateTimeOffset(
            now.Year, now.Month, now.Day,
            now.Hour, now.Minute, now.Second,
            now.Offset);
        ctx.Response.Headers.LastModified = newLastModified.ToString("R");
        LastModifiedRepo["weather-forecast"] = newLastModified;
        return NoCache();
    }

    private static StatusCodeHttpResult EmptyResultWith304NotModifiedStatus(HttpContext ctx)
    {
        ctx.Response.StatusCode = StatusCodes.Status304NotModified;
        ctx.Response.Headers.LastModified = LastModifiedRepo["weather-forecast"].ToString("R");
        return TypedResults.StatusCode(StatusCodes.Status304NotModified);
    }

    #endregion
}