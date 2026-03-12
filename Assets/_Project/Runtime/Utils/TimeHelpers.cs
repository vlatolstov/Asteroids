using System;

namespace _Project.Runtime.Utils
{
    public static class TimeHelpers
    {
        public static string FormatTimestamp(long unixMilliseconds)
        {
            if (unixMilliseconds <= 0)
            {
                return "No timestamp";
            }

            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds)
                    .ToLocalTime()
                    .ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (ArgumentOutOfRangeException)
            {
                return "Invalid timestamp";
            }
        }
    }
}