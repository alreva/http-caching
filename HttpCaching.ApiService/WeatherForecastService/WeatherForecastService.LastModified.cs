using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    public async Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > LastModified(HttpContext ctx)
    {
        if (!TryGetModifiedSinceFromHeaders(ctx.Request.Headers, out var ifModifiedSince))
        {
            return await NoCacheWithLastModifiedResponseHeader(ctx);
        }
        
        if (ModifiedLaterThanRequested(weatherOptions.LastModified, ifModifiedSince))
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
        return TryParseDate(requestHeaders.IfModifiedSince, out lastModified);
    }

    private static bool ModifiedLaterThanRequested(DateTimeOffset lastModified, DateTimeOffset ifModifiedSince)
    {
        return lastModified > ifModifiedSince;
    }

    private Task<Ok<WeatherForecast[]>> NoCacheWithLastModifiedResponseHeader(HttpContext ctx)
    {
        ctx.Response.Headers.LastModified = weatherOptions.LastModified.ToString("R");
        return NoCache();
    }

    private static bool TryParseDate(string? val, out DateTimeOffset parsed)
        => DateTimeOffset.TryParseExact(
            val,
            "R", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
            out parsed);

    #endregion
}