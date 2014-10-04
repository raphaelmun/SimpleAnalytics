using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines an occurance of an Event
    /// </summary>
    public struct EventOccurance
    {
        public DateTime Start;
        public DateTime Finished;
        public DateTime Expiration;

        public EventOccurance( DateTime time, DateTime finishedTime, DateTime expiration )
        {
            Start = time;
            Finished = finishedTime;
            Expiration = expiration;
        }

        /// <summary>
        /// Gets the length of time between Finished and Start
        /// </summary>
        [JsonIgnore]
        public TimeSpan TimeLength
        {
            get { return Finished - Start; }
        }

        /// <summary>
        /// Gets a boolean value indicating expiration
        /// </summary>
        [JsonIgnore]
        public bool IsExpired
        {
            get { return Finished >= Expiration; }
        }

        /// <summary>
        /// Converts a JSON string into an EventOccurance object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event occurance</param>
        /// <returns>The JSON as an EventOccurance object</returns>
        public static EventOccurance FromString( string occuranceString )
        {
            return JsonConvert.DeserializeObject<EventOccurance>( occuranceString );
        }

        /// <summary>
        /// Converts the EventOccurance into a JSON string
        /// </summary>
        /// <returns>The EventOccurance object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
        }
    }
}
