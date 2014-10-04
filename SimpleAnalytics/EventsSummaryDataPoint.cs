using System;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a data point of a summary of an Event
    /// </summary>
    public struct EventsSummaryDataPoint
    {
        public DateTime Time;
        public EventsSummary Summary;

        public EventsSummaryDataPoint( DateTime time, EventsSummary summary )
        {
            Time = time;
            Summary = summary;
        }

        /// <summary>
        /// Converts a JSON string into an EventsSummaryDataPoint object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event collection datapoint</param>
        /// <returns>The JSON as an EventsSummaryDataPoint object</returns>
        public static EventsSummaryDataPoint FromString( string occuranceString )
        {
            return JsonConvert.DeserializeObject<EventsSummaryDataPoint>( occuranceString );
        }

        /// <summary>
        /// Converts the EventsSummaryDataPoint into a JSON string
        /// </summary>
        /// <returns>The EventsSummaryDataPoint object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
        }
    }
}
