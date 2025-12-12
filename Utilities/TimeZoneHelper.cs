namespace MarutiTrainingPortal.Utilities
{
    public static class TimeZoneHelper
    {
        public static List<TimeZoneViewModel> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => new TimeZoneViewModel
                {
                    Id = tz.Id,
                    DisplayName = $"(UTC{tz.BaseUtcOffset:hh\\:mm}) {tz.DisplayName}"
                })
                .OrderBy(tz => tz.DisplayName)
                .ToList();
        }

        public static string GetUserTimeZone()
        {
            // Default to UTC, can be enhanced to detect user timezone from browser
            return "UTC";
        }
    }

    public class TimeZoneViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
