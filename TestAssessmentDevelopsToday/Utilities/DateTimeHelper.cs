using CsvHelper;
using System.Globalization;

namespace TestAssessmentDevelopsToday.Utilities
{
    public static class DateTimeHelper
    {
        public static DateTime? TryParseDate(CsvReader csv, string columnName)
        {
            var dateStr = csv.GetField<string>(columnName)?.Trim();

            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return null;
            }

            if (DateTime.TryParseExact(dateStr, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime date))
            {
                try
                {
                    TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    return TimeZoneInfo.ConvertTimeToUtc(date, estZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine("Warning: Timezone 'Eastern Standard Time' not found. Using local UTC conversion.");
                    return date.ToUniversalTime();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting time: {ex.Message}");
                    return null;
                }
            }

            return null;
        }
    }
}
