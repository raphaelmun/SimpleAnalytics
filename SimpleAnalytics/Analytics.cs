﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleAnalytics
{
    public class Analytics
    {
        static Events events = new Events();

        /// <summary>
        /// Global static Events instance for easy use
        /// </summary>
        public static Events Events { get { return events; } }
    }
}
