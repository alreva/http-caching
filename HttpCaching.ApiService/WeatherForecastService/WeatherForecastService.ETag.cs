using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    public async Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > ETag(HttpContext ctx)
    {
        if (!TryGetIfNoneMatchFromHeaders(ctx.Request.Headers, out var eTag))
        {
            return await NoCacheWithETagResponseHeader(ctx);
        }
        
        if (weatherOptions.ETag != eTag)
        {
            return await NoCacheWithETagResponseHeader(ctx);
        }

        return EmptyResultWith304NotModifiedStatus(ctx);
    }

    #region Private methods

    private static bool TryGetIfNoneMatchFromHeaders(
        IHeaderDictionary requestHeaders,
        out string eTag)
    {
        eTag = (string?)requestHeaders.IfNoneMatch ?? "";
        return (string?)requestHeaders.IfNoneMatch != null;
    }

    private Task<Ok<WeatherForecast[]>> NoCacheWithETagResponseHeader(HttpContext ctx)
    {
        ctx.Response.Headers.ETag = weatherOptions.ETag;
        return NoCache();
    }

    #endregion
}