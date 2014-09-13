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

    public class Events
    {
        public const string DefaultName = "Default";
        public const int DefaultExpirationTime = 300;
        Dictionary<string, Event> events;
        Dictionary<string, string> properties;

        public string Name
        {
            get
            {
                return properties[ "Name" ];
            }
            set
            {
                properties[ "Name" ] = value;
            }
        }

        public Dictionary<string, string> Details
        {
            get
            {
                return properties;
            }
            set
            {
                Detail( value );
            }
        }

        public Event this[ string eventName ]
        {
            get
            {
                if( !events.ContainsKey( eventName ) )
                {
                    return null;
                }
                return events[ eventName ];
            }
        }

        public Events()
        {
            events = new Dictionary<string, Event>();
            properties = new Dictionary<string, string>();
            properties.Add( "Name", DefaultName );
        }

        public Events( Dictionary<string, string> details )
        {
            events = new Dictionary<string, Event>();
            properties = details;
            if( !details.ContainsKey( "Name" ) )
            {
                properties.Add( "Name", DefaultName );
            }
        }

        public void Detail( string key, string value )
        {
            if( properties.ContainsKey( key ) )
            {
                properties[ key ] = value;
            }
            else
            {
                properties.Add( key, value );
            }
        }

        public void Detail( Dictionary<string, string> details )
        {
            foreach( string key in details.Keys )
            {
                if( properties.ContainsKey( key ) )
                {
                    properties[ key ] = details[ key ];
                }
                else
                {
                    properties.Add( key, details[ key ] );
                }
            }
        }

        public void Increment( string eventName )
        {
            if( !events.ContainsKey( eventName ) )
            {
                events.Add( eventName, new Event() );
            }
            events[ eventName ].Increment();
        }

        public string Open( string eventName, int expirationInSeconds = DefaultExpirationTime )
        {
            if( !events.ContainsKey( eventName ) )
            {
                events.Add( eventName, new Event() );
            }
            string uuid = Utility.GenerateUUID();
            events[ eventName ].Open( uuid, expirationInSeconds );
            return uuid;
        }

        public void Close( string eventName, string eventID )
        {
            if( !events[ eventName ].Close( eventID ) )
            {
                throw new ArgumentException( "Bad Event ID" );
            }
        }

        public void Flush()
        {
            foreach( string key in events.Keys )
            {
                events[ key ].Flush();
            }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
