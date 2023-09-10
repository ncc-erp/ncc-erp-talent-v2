using Abp.Timing;
using System;
using System.Globalization;
using TalentV2.ModelExtends;

namespace TalentV2.Utils
{
    public class DateTimeUtils
    {
        public const long TOTAL_MILLIS_IN_DAY = 86400000;
        public static DateTime LocalFirstDay1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime().AddHours(7);

        // All now function use Clock.Provider.Now
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }

        public static bool CustomTryParseExact(string s, string format, out DateTime result)
        {
            return DateTime.TryParseExact(s, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result);
        }

        public static DateTime DateTimeFromMilliseconds(long millis)
        {
            return LocalFirstDay1970.AddMilliseconds(millis);
        }
        public static string ToString(DateTime? dateTime)
        {
            if(!dateTime.HasValue) return string.Empty;
            return dateTime.Value.ToString("yyyy/MM/dd");
        }
        public static string ToStringddMMyyyy(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;
            return dateTime.Value.ToString("dd/MM/yyyy");
        }

        public static string ToddMMyyyyHHmm(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;
            return dateTime.Value.ToString("dd/MM/yyyy HH:mm");
        }
        public static string GetTime(DateTime? dateTime)
        {
            if(!dateTime.HasValue) return string.Empty ;
            return dateTime.Value.ToString("HH:mm");
        }
        public static long GetTickTime(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;
            return time.Ticks;
        }
        public static DayOfWeekDto GetDay(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return new DayOfWeekDto();
            var day = dateTime.Value.DayOfWeek;
            return CommonUtils.DayOfWeeks.Find(s => s.DayOfWeek == day);
        }
    }
}
