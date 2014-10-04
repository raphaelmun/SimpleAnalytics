using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a history of Event collection summaries
    /// </summary>
    public struct EventsHistory
    {
        public const int MaxSummariesTracked = 100;
        public Dictionary<string, string> Details;
        public Dictionary<string, EventsSummaryDataPoint[]> Events;

        public EventsHistory( Dictionary<string, string> details, Dictionary<string, EventsSummaryDataPoint[]> events )
        {
            if( details == null )
            {
                Details = new Dictionary<string, string>();
            }
            else
            {
                Details = details;
            }
            if( events == null )
            {
                Events = new Dictionary<string, EventsSummaryDataPoint[]>();
            }
            else
            {
                Events = events;
            }
        }

        /// <summary>
        /// Converts a JSON string into an EventsHistory object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event collection history</param>
        /// <returns>The JSON as an EventsHistory object</returns>
        public static EventsHistory FromString( string historyString )
        {
            return JsonConvert.DeserializeObject<EventsHistory>( historyString );
        }

        /// <summary>
        /// Converts the EventsHistory into a JSON string
        /// </summary>
        /// <returns>The EventsHistory object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
        }
    }
}
