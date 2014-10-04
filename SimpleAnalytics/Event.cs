using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a trackable event
    /// </summary>
    public class Event
    {
        public const int MaxOccurancesTracked = 100;
        EventOccurance[] occurances;
        Dictionary<string, EventOccurance> openOccurances;
        int currentIndex = 0;
        int count = 0;
        int expiredCount = 0;
        float averageTimeLength = 0;

        /// <summary>
        /// Constructor for Event
        /// </summary>
        public Event()
        {
            occurances = new EventOccurance[ MaxOccurancesTracked ];
            openOccurances = new Dictionary<string, EventOccurance>();
            Reset();
        }

        /// <summary>
        /// Gets the number of times the event has occured
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the number of currently open event occurances
        /// </summary>
        public int OpenCount
        {
            get { return openOccurances.Count; }
        }

        /// <summary>
        /// Gets the number of expired event occurances
        /// </summary>
        public int ExpiredCount
        {
            get { return expiredCount; }
        }

        /// <summary>
        /// Gets the average time between an event's Open() and Close()
        /// </summary>
        public float AverageTimeLength
        {
            get { return averageTimeLength; }
        }

        /// <summary>
        /// Gets the Event summary as a JSON string
        /// </summary>
        [JsonIgnore]
        public EventSummary Summary
        {
            get { return new EventSummary( Count, OpenCount, ExpiredCount, AverageTimeLength ); }
        }

        /// <summary>
        /// Gets a copy of the array of all occurances of the event
        /// </summary>
        public EventOccurance[] Occurances
        {
            get
            {
                EventOccurance[] occurancesCopy;
                if( Count < MaxOccurancesTracked )
                {
                    occurancesCopy = new EventOccurance[ Count ];
                    Array.Copy( occurances, occurancesCopy, Count );
                }
                else
                {
                    occurancesCopy = (EventOccurance[])occurances.Clone();
                }
                return occurancesCopy;
            }
            set
            {
                count = 0;
                for( int i = 0; i < value.Length && i < MaxOccurancesTracked; i++ )
                {
                    occurances[ i ] = value[ i ];
                    count++;
                }
                calculateAverageTimeLength();
            }
        }

        /// <summary>
        /// Adds an occurance of the event to the total
        /// </summary>
        public void Increment()
        {
            occurances[ currentIndex ] = new EventOccurance( SystemTime.UtcNow, SystemTime.UtcNow, SystemTime.UtcNow );
            currentIndex = ( currentIndex + 1 ) % MaxOccurancesTracked;
            count++;
        }

        /// <summary>
        /// Opens an occurance of the event for tracking
        /// </summary>
        /// <param name="eventId">Unique ID to track the event occurance. This should be a GUID in string format.</param>
        /// <param name="expirationInSeconds">Time it takes for this event occurance to expire during a Flush()</param>
        /// <returns>True on success, false otherwise.</returns>
        public bool Open( string eventId, int expirationInSeconds )
        {
            if( openOccurances.ContainsKey( eventId ) )
            {
                openOccurances[ eventId ] = new EventOccurance( SystemTime.UtcNow, DateTime.MinValue, SystemTime.UtcNow + TimeSpan.FromSeconds( expirationInSeconds ) );
            }
            else
            {
                openOccurances.Add( eventId, new EventOccurance( SystemTime.UtcNow, DateTime.MinValue, SystemTime.UtcNow + TimeSpan.FromSeconds( expirationInSeconds ) ) );
            }
            return true;
        }

        /// <summary>
        /// Completes the tracking for the open event occurance and calculates the average time length for the event.
        /// </summary>
        /// <param name="eventId">Unique ID to track the event occurance. This should be a GUID in string format.</param>
        /// <returns>True on success, false otherwise.</returns>
        public bool Close( string eventId )
        {
            if( openOccurances.ContainsKey( eventId ) )
            {
                EventOccurance occurance = openOccurances[ eventId ];
                occurances[ currentIndex ] = new EventOccurance( occurance.Start, SystemTime.UtcNow, DateTime.MaxValue );
                currentIndex = ( currentIndex + 1 ) % MaxOccurancesTracked;
                count++;
                openOccurances.Remove( eventId );
                calculateAverageTimeLength();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets all values of the event
        /// </summary>
        public void Reset()
        {
            openOccurances.Clear();
            currentIndex = 0;
            count = 0;
            expiredCount = 0;
            averageTimeLength = 0;
        }

        /// <summary>
        /// Removes all expired event occurances
        /// </summary>
        public void Flush()
        {
            DateTime now = SystemTime.UtcNow;
            List<string> expiringOccurances = new List<string>();
            foreach( string key in openOccurances.Keys )
            {
                if( openOccurances[ key ].Expiration <= now )
                {
                    EventOccurance occurance = openOccurances[ key ];
                    occurances[ currentIndex ] = new EventOccurance( occurance.Start, now, occurance.Expiration );
                    currentIndex = ( currentIndex + 1 ) % MaxOccurancesTracked;
                    count++;
                    expiringOccurances.Add( key );
                    expiredCount++;
                }
            }
            foreach( string key in expiringOccurances )
            {
                openOccurances.Remove( key );
            }
        }

        /// <summary>
        /// Converts a JSON string into an Event object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event</param>
        /// <returns>The JSON as an Even object</returns>
        public static Event FromString( string eventString )
        {
            return JsonConvert.DeserializeObject<Event>( eventString );
        }

        /// <summary>
        /// Converts the Event into a JSON string
        /// </summary>
        /// <returns>The Event object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
            //StringBuilder sb = new StringBuilder();
            //bool passedFirstKey = false;
            //sb.AppendFormat( @"{{""Count"":{0},""Open"":{1},""Exp"":{2},""AvgTime"":{3},""Occurances"":[", Count, OpenCount, ExpiredCount, AverageTimeLength );
            //for( int i = 0; i < Count && i < MaxOccurancesTracked; i++ )
            //{
            //    if( passedFirstKey )
            //    {
            //        sb.Append( "," );
            //    }
            //    sb.AppendFormat( occurances[ i ].ToString() );
            //    passedFirstKey = true;
            //}
            //sb.AppendFormat( @"]" );
            //sb.AppendFormat( "}}" );
            //return sb.ToString();
        }

        private void calculateAverageTimeLength()
        {
            // Calculate Average Time Length (only for non-expired events)
            float timeTotal = 0.0f;
            int occurancesTotal = 0;
            for( int i = 0; i < Count && i < MaxOccurancesTracked; i++ )
            {
                if( !occurances[ i ].IsExpired )
                {
                    timeTotal += (float)occurances[ i ].TimeLength.TotalSeconds;
                    occurancesTotal++;
                }
            }
            averageTimeLength = timeTotal / occurancesTotal;
        }
    }
}
