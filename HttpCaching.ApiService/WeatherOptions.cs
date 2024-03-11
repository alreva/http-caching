namespace HttpCaching.ApiService;

public class WeatherOptions
{
    public TimeSpan Wait { get; set; } = default;
    public DateTimeOffset LastModified { get; set; } = default;
    public int MaxAge { get; set; } = default;
    public string ETag { get; set; }
}