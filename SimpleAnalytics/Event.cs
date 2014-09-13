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
        List<EventOccurance> occurances;
        Dictionary<string, EventOccurance> openOccurances;
        int expiredCount;

        public Event()
        {
            occurances = new List<EventOccurance>();
            openOccurances = new Dictionary<string, EventOccurance>();
            expiredCount = 0;
        }

        public int Count
        {
            get { return occurances.Count; }
        }

        public int OpenCount
        {
            get { return openOccurances.Count; }
        }

        public int ExpiredCount
        {
            get { return expiredCount; }
        }

        public List<EventOccurance> Occurances
        {
            get { return occurances; }
        }

        public void Increment()
        {
            occurances.Add( new EventOccurance( SystemTime.UtcNow, SystemTime.UtcNow, SystemTime.UtcNow ) );
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
                occurances.Add( new EventOccurance( occurance.Time, SystemTime.UtcNow, occurance.Expiration ) );
                openOccurances.Remove( eventId );
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
                    occurances.Add( new EventOccurance( occurance.Time, now, occurance.Expiration ) );
                    expiringOccurances.Add( key );
                    expiredCount++;
                }
            }
            foreach( string key in expiringOccurances )
            {
                openOccurances.Remove( key );
            }
        }
    }
}
