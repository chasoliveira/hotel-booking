using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class DateTimeExtensions
{
  public static DateTime SetLastHourOfTheDay(this DateTime dateTime)
    => dateTime.Date.AddDays(1).AddSeconds(-1);
}