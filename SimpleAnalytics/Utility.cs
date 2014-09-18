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
            get
            {
                if( UtcNowFunc != null )
                {
                    return UtcNowFunc().ToLocalTime();
                }
                return DateTime.Now;
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                if( UtcNowFunc != null )
                {
                    return UtcNowFunc();
                }
                return DateTime.UtcNow;
            }
        }

        public static Func<DateTime> UtcNowFunc = null;

        public static void ResetToDefault()
        {
            UtcNowFunc = null;
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
