using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a summary of an Event collection
    /// </summary>
    public struct EventsSummary
    {
        public Dictionary<string, string> Details;
        public Dictionary<string, EventSummary> Events;

        public EventsSummary( Dictionary<string, string> details, Dictionary<string, Event> events )
        {
            if( details == null )
            {
                Details = new Dictionary<string, string>();
            }
            else
            {
                Details = details;
            }
            Events = new Dictionary<string, EventSummary>();
            if( events != null )
            {
                foreach( string key in events.Keys )
                {
                    Events.Add( key, events[ key ].Summary );
                }
            }
        }

        /// <summary>
        /// Converts a JSON string into an EventsSummary object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event summary</param>
        /// <returns>The JSON as an EventsSummary object</returns>
        public static EventsSummary FromString( string summaryString )
        {
            return JsonConvert.DeserializeObject<EventsSummary>( summaryString );
        }

        /// <summary>
        /// Converts the EventsSummary into a JSON string
        /// </summary>
        /// <returns>The EventsSummary object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
        }
    }
}
