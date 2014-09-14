using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleAnalytics
{
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
