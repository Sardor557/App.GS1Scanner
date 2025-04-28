namespace App.Utils
{
    public static class DateUtils
    {
        public static DateTime Date()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.UtcNow;
            DateTimeOffset usersTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            return usersTime.DateTime;
        }

        public static long ToNowTimeSpan()
        {
            return (long)(Date() - new DateTime(1970, 1, 1)).TotalSeconds * 1000;
        }

        public static long ToNowHowMuch(DateTime date)
        {
            TimeSpan time = date - Date();
            return (long)time.TotalSeconds * 1000;
        }

        public static long ToTimeSpan(DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 1000;
        }

        public static DateTime TimeSpanToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
