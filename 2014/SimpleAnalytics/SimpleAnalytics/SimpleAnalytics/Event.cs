using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleAnalytics
{
    public struct EventOccurance
    {
        public DateTime Time;
        public DateTime FinishedTime;
        public DateTime Expiration;

        public EventOccurance( DateTime time, DateTime finishedTime, DateTime expiration )
        {
            Time = time;
            FinishedTime = finishedTime;
            Expiration = expiration;
        }

        public TimeSpan TimeLength
        {
            get { return FinishedTime - Time; }
        }

        public bool IsExpired
        {
            get { return FinishedTime >= Expiration; }
        }
    }

    public class Event
    {
        public const int MaxOccurancesTracked = 100;
        EventOccurance[] occurances;
        Dictionary<string, EventOccurance> openOccurances;
        int currentIndex = 0;
        int count = 0;
        int expiredCount = 0;
        float averageTimeLength = 0;

        public Event()
        {
            occurances = new EventOccurance[ MaxOccurancesTracked ];
            openOccurances = new Dictionary<string, EventOccurance>();
            currentIndex = 0;
            count = 0;
            expiredCount = 0;
            averageTimeLength = 0;
        }

        public int Count
        {
            get { return count; }
        }

        public int OpenCount
        {
            get { return openOccurances.Count; }
        }

        public int ExpiredCount
        {
            get { return expiredCount; }
        }

        public float AverageTimeLength
        {
            get { return averageTimeLength; }
        }

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
        }

        public void Increment()
        {
            occurances[ currentIndex ] = new EventOccurance( SystemTime.UtcNow, SystemTime.UtcNow, SystemTime.UtcNow );
            currentIndex = ( currentIndex + 1 ) % MaxOccurancesTracked;
            count++;
        }

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

        public bool Close( string eventId )
        {
            if( openOccurances.ContainsKey( eventId ) )
            {
                EventOccurance occurance = openOccurances[ eventId ];
                occurances[ currentIndex ] = new EventOccurance( occurance.Time, SystemTime.UtcNow, occurance.Expiration );
                currentIndex = ( currentIndex + 1 ) % MaxOccurancesTracked;
                count++;
                openOccurances.Remove( eventId );
                // Calculate Average Time Length
                float timeTotal = 0.0f;
                for( int i = 0; i < Count && i < MaxOccurancesTracked; i++ )
                {
                    timeTotal += (float)occurances[ i ].TimeLength.TotalSeconds;
                }
                if( Count < MaxOccurancesTracked )
                {
                    averageTimeLength = timeTotal / Count;
                }
                else
                {
                    averageTimeLength = timeTotal / MaxOccurancesTracked;
                }
                return true;
            }
            return false;
        }

        public void Flush()
        {
            DateTime now = SystemTime.UtcNow;
            List<string> expiringOccurances = new List<string>();
            foreach( string key in openOccurances.Keys )
            {
                if( openOccurances[ key ].Expiration <= now )
                {
                    EventOccurance occurance = openOccurances[ key ];
                    occurances[ currentIndex ] = new EventOccurance( occurance.Time, now, occurance.Expiration );
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

        public override string ToString()
        {
            return string.Format( @"{{""Count"":{0},""Open"":{1},""Expired"":{2},""AverageTimeLength"":{3}}}", Count, OpenCount, ExpiredCount, AverageTimeLength );
        }
    }
}
