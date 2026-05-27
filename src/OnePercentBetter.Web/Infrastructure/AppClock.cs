namespace OnePercentBetter.Web.Infrastructure;

public static class AppClock
{
    private static readonly object Sync = new();
    private static TimeZoneInfo _timeZone = TimeZoneInfo.Utc;

    public static string TimeZoneId
    {
        get
        {
            lock (Sync)
            {
                return _timeZone.Id;
            }
        }
    }

    public static DateTime UtcNow => DateTime.UtcNow;

    public static DateTime LocalNow
    {
        get
        {
            lock (Sync)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
            }
        }
    }

    public static DateTime Today => LocalNow.Date;

    public static void Configure(string? configuredTimeZoneId)
    {
        var resolved = ResolveTimeZone(configuredTimeZoneId);

        lock (Sync)
        {
            _timeZone = resolved;
        }
    }

    private static TimeZoneInfo ResolveTimeZone(string? configuredTimeZoneId)
    {
        var candidates = new[]
        {
            configuredTimeZoneId,
            Environment.GetEnvironmentVariable("APP_TIME_ZONE"),
            Environment.GetEnvironmentVariable("TZ"),
            "America/Sao_Paulo",
            "E. South America Standard Time",
            "UTC"
        };

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(candidate.Trim());
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }
}
