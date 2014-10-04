using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a collection of trackable events
    /// </summary>
    public class Events
    {
        public const string DefaultName = "Default";
        public const int DefaultExpirationTime = 300;
        Dictionary<string, Event> events;
        Dictionary<string, string> properties;

        /// <summary>
        /// Gets or Sets the Name of the events set
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the Detail properties of the events set
        /// </summary>
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

        /// <summary>
        /// Gets the Event object mapped to the name of the event
        /// </summary>
        /// <param name="eventName">A name string used to track the event</param>
        /// <returns>The Event object mapped to the name of the event</returns>
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

        /// <summary>
        /// Gets the Events summary as a JSON string
        /// </summary>
        [JsonIgnore]
        public EventsSummary Summary
        {
            get
            {
                return new EventsSummary( properties, events );
                //StringBuilder sb = new StringBuilder();
                //sb.AppendFormat( "{{" );
                //bool passedFirstKey = false;
                //foreach( string key in Details.Keys )
                //{
                //    if( passedFirstKey )
                //    {
                //        sb.Append( "," );
                //    }
                //    sb.AppendFormat( @"""{0}"":""{1}""", key, Details[ key ] );
                //    passedFirstKey = true;
                //}
                //sb.AppendFormat( @",""Events"":[" );
                //passedFirstKey = false;
                //foreach( string key in events.Keys )
                //{
                //    if( passedFirstKey )
                //    {
                //        sb.Append( "," );
                //    }
                //    sb.AppendFormat( @"{{""{0}"":{1}}}", key, events[ key ].Summary );
                //    passedFirstKey = true;
                //}
                //sb.AppendFormat( @"]" );
                //sb.AppendFormat( "}}" );
                //return sb.ToString();
            }
        }

        /// <summary>
        /// Constructor for the Events class
        /// </summary>
        public Events()
        {
            events = new Dictionary<string, Event>();
            properties = new Dictionary<string, string>();
            properties.Add( "Name", DefaultName );
        }

        /// <summary>
        /// Constructor for the Events class
        /// </summary>
        /// <param name="details">Detail properties of the events set</param>
        public Events( Dictionary<string, string> details )
        {
            events = new Dictionary<string, Event>();
            properties = details;
            if( !details.ContainsKey( "Name" ) )
            {
                properties.Add( "Name", DefaultName );
            }
        }

        /// <summary>
        /// Sets this event set's detail property
        /// </summary>
        /// <param name="key">Detail property key</param>
        /// <param name="value">Detail property value</param>
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

        /// <summary>
        /// Sets this event set's detail properties
        /// </summary>
        /// <param name="details">Detail properties of the events set</param>
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

        /// <summary>
        /// Adds an occurance of the event to the total
        /// </summary>
        /// <param name="eventName">A name string used to track the event</param>
        public void Increment( string eventName )
        {
            if( !events.ContainsKey( eventName ) )
            {
                events.Add( eventName, new Event() );
            }
            events[ eventName ].Increment();
        }

        /// <summary>
        /// Opens an occurance of the event for tracking
        /// </summary>
        /// <param name="eventName">A name string used to track the event</param>
        /// <param name="expirationInSeconds">Time it takes for this event occurance to expire during a Flush()</param>
        /// <returns>Unique ID to track the event occurance as a GUID in string format.</returns>
        public string Open( string eventName, int expirationInSeconds = DefaultExpirationTime )
        {
            return Open( eventName, Utility.GenerateUUID(), expirationInSeconds );
        }

        /// <summary>
        /// Opens an occurance of the event for tracking
        /// </summary>
        /// <param name="eventName">A name string used to track the event</param>
        /// <param name="uuid">Unique ID to track the event occurance as a GUID in string format</param>
        /// <param name="expirationInSeconds">Time it takes for this event occurance to expire during a Flush()</param>
        /// <returns>Unique ID to track the event occurance as a GUID in string format.</returns>
        public string Open( string eventName, string uuid, int expirationInSeconds = DefaultExpirationTime )
        {
            if( !events.ContainsKey( eventName ) )
            {
                events.Add( eventName, new Event() );
            }
            events[ eventName ].Open( uuid, expirationInSeconds );
            return uuid;
        }

        /// <summary>
        /// Completes the tracking for the open event occurance and calculates the average time length for the event.
        /// </summary>
        /// <param name="eventName">A name string used to track the event</param>
        /// <param name="eventID">Unique ID to track the event occurance. This should be a GUID in string format.</param>
        /// <returns>True on success, false otherwise.</returns>
        public bool Close( string eventName, string eventID )
        {
            if( events.ContainsKey( eventName ) )
            {
                return events[ eventName ].Close( eventID );
            }
            return false;
        }

        /// <summary>
        /// Resets all values of the events
        /// </summary>
        public void Reset()
        {
            foreach( string key in events.Keys )
            {
                events[ key ].Reset();
            }
        }

        /// <summary>
        /// Removes all expired occurances in the set of events
        /// </summary>
        public void Flush()
        {
            foreach( string key in events.Keys )
            {
                events[ key ].Flush();
            }
        }

        /// <summary>
        /// Converts the set of events into a JSON string
        /// </summary>
        /// <returns>The set of events as a JSON string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "{{" );
            bool passedFirstKey = false;
            foreach( string key in Details.Keys )
            {
                if( passedFirstKey )
                {
                    sb.Append( "," );
                }
                sb.AppendFormat( @"""{0}"":""{1}""", key, Details[ key ] );
                passedFirstKey = true;
            }
            sb.AppendFormat( @",""Events"":[" );
            passedFirstKey = false;
            foreach( string key in events.Keys )
            {
                if( passedFirstKey )
                {
                    sb.Append( "," );
                }
                sb.AppendFormat( @"{{""{0}"":{1}}}", key, events[ key ].ToString() );
                passedFirstKey = true;
            }
            sb.AppendFormat( @"]" );
            sb.AppendFormat( "}}" );
            return sb.ToString();
        }
    }
}
