using System;
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
        public Dictionary<string, int> Counts;
        private Dictionary<string, int> indices;
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
                Counts = new Dictionary<string, int>();
                indices = new Dictionary<string, int>();
                Min = new Dictionary<string, EventSummary>();
                Avg = new Dictionary<string, EventSummary>();
                Max = new Dictionary<string, EventSummary>();
            }
            else
            {
                Events = new Dictionary<string, EventsSummaryDataPoint[]>();
                Counts = new Dictionary<string, int>();
                indices = new Dictionary<string, int>();
                Min = new Dictionary<string, EventSummary>();
                Avg = new Dictionary<string, EventSummary>();
                Max = new Dictionary<string, EventSummary>();
                foreach( string key in events.Keys )
                {
                    Events.Add( key, new EventsSummaryDataPoint[ MaxSummariesTracked ] );
                    Counts.Add( key, 0 );
                    for( int i = 0; i < events[ key ].Length && i < MaxSummariesTracked; i++ )
                    {
                        Events[ key ][ i ] = events[ key ][ i ];
                        Counts[ key ]++;
                    }
                    indices.Add( key, Counts[ key ] % MaxSummariesTracked );
                    EventsSummaryDataPoint[] dataPoints = Events[ key ];
                    Min.Add( key, dataPoints[ 0 ].Summary );
                    Max.Add( key, dataPoints[ 0 ].Summary );
                    Avg.Add( key, dataPoints[ 0 ].Summary );
                }
                recalculateStats();
            }
        }

        // TODO: Add tests
        public EventsHistory( EventsSummary summary )
        {
            Details = summary.Details;
            Events = new Dictionary<string, EventsSummaryDataPoint[]>();
            Counts = new Dictionary<string, int>();
            indices = new Dictionary<string, int>();
            Min = new Dictionary<string, EventSummary>();
            Avg = new Dictionary<string, EventSummary>();
            Max = new Dictionary<string, EventSummary>();
            DateTime timeStamp = SystemTime.UtcNow;
            foreach( string key in summary.Events.Keys )
            {
                Events.Add( key, new EventsSummaryDataPoint[ MaxSummariesTracked ] );
                Events[ key ][ 0 ] = new EventsSummaryDataPoint( timeStamp, summary.Events[ key ] );
                Counts.Add( key, 1 );
                indices.Add( key, Counts[ key ] % MaxSummariesTracked );
                EventsSummaryDataPoint[] dataPoints = Events[ key ];
                Min.Add( key, dataPoints[ 0 ].Summary );
                Max.Add( key, dataPoints[ 0 ].Summary );
                Avg.Add( key, dataPoints[ 0 ].Summary );
            }
            recalculateStats();
        }

        /// <summary>
        /// Adds an EventsSummary to the history
        /// </summary>
        /// <param name="time">Timestamp of the summary</param>
        /// <param name="summary">The summary of the event collection</param>
        public void AddSummary( DateTime time, EventsSummary summary )
        {
            // TODO: Compare and merge details
            //Details = summary.Details;
            if( summary.Events != null )
            {
                foreach( string key in summary.Events.Keys )
                {
                    EventsSummaryDataPoint dataPoint = new EventsSummaryDataPoint( time, summary.Events[ key ] );
                    if( Events.ContainsKey( key ) )
                    {
                        Events[ key ][ indices[ key ] ] = dataPoint;
                        Counts[ key ]++;
                        indices[ key ] = Counts[ key ] % MaxSummariesTracked;
                    }
                    else
                    {
                        // New event key
                        Events.Add( key, new EventsSummaryDataPoint[ MaxSummariesTracked ] );
                        Events[ key ][ 0 ] = dataPoint;
                        Counts.Add( key, 1 );
                        indices.Add( key, Counts[ key ] % MaxSummariesTracked );
                        EventsSummaryDataPoint[] dataPoints = Events[ key ];
                        Min.Add( key, dataPoints[ 0 ].Summary );
                        Max.Add( key, dataPoints[ 0 ].Summary );
                        Avg.Add( key, dataPoints[ 0 ].Summary );
                    }
                }
                recalculateStats();
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

        private void recalculateStats()
        {
            foreach( string key in Events.Keys )
            {
                EventsSummaryDataPoint[] dataPoints = Events[ key ];
                EventSummary min = dataPoints[ 0 ].Summary;
                EventSummary max = dataPoints[ 0 ].Summary;
                EventSummary avg = new EventSummary( 0, 0, 0, 0.0f );
                int dataPointCount = 0;
                for( int i = 0; i < Counts[ key ] && i < dataPoints.Length; i++ )
                {
                    if( dataPoints[ i ].Summary.Count < min.Count ) { min.Count = dataPoints[ i ].Summary.Count; }
                    if( dataPoints[ i ].Summary.Open < min.Open ) { min.Open = dataPoints[ i ].Summary.Open; }
                    if( dataPoints[ i ].Summary.Expired < min.Expired ) { min.Expired = dataPoints[ i ].Summary.Expired; }
                    if( dataPoints[ i ].Summary.AverageTime < min.AverageTime ) { min.AverageTime = dataPoints[ i ].Summary.AverageTime; }

                    if( dataPoints[ i ].Summary.Count > max.Count ) { max.Count = dataPoints[ i ].Summary.Count; }
                    if( dataPoints[ i ].Summary.Open > max.Open ) { max.Open = dataPoints[ i ].Summary.Open; }
                    if( dataPoints[ i ].Summary.Expired > max.Expired ) { max.Expired = dataPoints[ i ].Summary.Expired; }
                    if( dataPoints[ i ].Summary.AverageTime > max.AverageTime ) { max.AverageTime = dataPoints[ i ].Summary.AverageTime; }

                    avg.Count += dataPoints[ i ].Summary.Count;
                    avg.Open += dataPoints[ i ].Summary.Open;
                    avg.Expired += dataPoints[ i ].Summary.Expired;
                    avg.AverageTime += dataPoints[ i ].Summary.AverageTime;
                    dataPointCount++;
                }

                avg.Count /= dataPointCount;
                avg.Open /= dataPointCount;
                avg.Expired /= dataPointCount;
                avg.AverageTime /= (float)dataPointCount;
                Min[ key ] = min;
                Max[ key ] = max;
                Avg[ key ] = avg;
            }
        }
    }
}
