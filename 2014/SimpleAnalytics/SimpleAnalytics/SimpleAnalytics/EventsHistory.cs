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
        public Dictionary<string, EventSummary> Min; // TODO: Add tests
        public Dictionary<string, EventSummary> Avg; // TODO: Add tests
        public Dictionary<string, EventSummary> Max; // TODO: Add tests

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
                Min = new Dictionary<string, EventSummary>();
                Avg = new Dictionary<string, EventSummary>();
                Max = new Dictionary<string, EventSummary>();
            }
            else
            {
                Events = events;
                Min = new Dictionary<string, EventSummary>();
                Avg = new Dictionary<string, EventSummary>();
                Max = new Dictionary<string, EventSummary>();
                foreach( string key in events.Keys )
                {
                    EventsSummaryDataPoint[] dataPoints = events[ key ];
                    EventSummary min = dataPoints[ 0 ].Summary;
                    EventSummary max = dataPoints[ 0 ].Summary;
                    EventSummary avg = new EventSummary( 0, 0, 0, 0.0f );
                    int dataPointCount = 0;
                    foreach( EventsSummaryDataPoint dataPoint in dataPoints )
                    {
                        if( dataPoint.Summary.Count < min.Count ) { min.Count = dataPoint.Summary.Count; }
                        if( dataPoint.Summary.Open < min.Open ) { min.Open = dataPoint.Summary.Open; }
                        if( dataPoint.Summary.Expired < min.Expired ) { min.Expired = dataPoint.Summary.Expired; }
                        if( dataPoint.Summary.AverageTime < min.AverageTime ) { min.AverageTime = dataPoint.Summary.AverageTime; }

                        if( dataPoint.Summary.Count > max.Count ) { max.Count = dataPoint.Summary.Count; }
                        if( dataPoint.Summary.Open > max.Open ) { max.Open = dataPoint.Summary.Open; }
                        if( dataPoint.Summary.Expired > max.Expired ) { max.Expired = dataPoint.Summary.Expired; }
                        if( dataPoint.Summary.AverageTime > max.AverageTime ) { max.AverageTime = dataPoint.Summary.AverageTime; }

                        avg.Count += dataPoint.Summary.Count;
                        avg.Open += dataPoint.Summary.Open;
                        avg.Expired += dataPoint.Summary.Expired;
                        avg.AverageTime += dataPoint.Summary.AverageTime;
                        dataPointCount++;
                    }

                    avg.Count /= dataPointCount;
                    avg.Open /= dataPointCount;
                    avg.Expired /= dataPointCount;
                    avg.AverageTime /= (float)dataPointCount;
                    Min.Add( key, min );
                    Max.Add( key, max );
                    Avg.Add( key, avg );
                }
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
