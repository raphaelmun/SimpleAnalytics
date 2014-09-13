using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleAnalytics
{
    public static class SystemTime
    {
        public static DateTime Now
        {
            get { return NowFunc(); }
        }

        public static DateTime UtcNow
        {
            get { return UtcNowFunc(); }
        }

        public static Func<DateTime> NowFunc = () => DateTime.Now;
        public static Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;

        public static void ResetToDefault()
        {
            Func<DateTime> NowFunc = () => DateTime.Now;
            Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;
        }
    }

    public static class Utility
    {
        public static string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
