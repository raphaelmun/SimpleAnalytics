using Newtonsoft.Json;

namespace SimpleAnalytics
{
    /// <summary>
    /// Defines a summary of an Event
    /// </summary>
    public struct EventSummary
    {
        public int Count;
        public int Open;
        public int Expired;
        public float AverageTime;

        public EventSummary( int count, int open, int expired, float averageTime )
        {
            Count = count;
            Open = open;
            Expired = expired;
            AverageTime = averageTime;
        }

        /// <summary>
        /// Converts a JSON string into an EventSummary object
        /// </summary>
        /// <param name="occuranceString">A JSON string of the event summary</param>
        /// <returns>The JSON as an EventSummary object</returns>
        public static EventSummary FromString( string summaryString )
        {
            return JsonConvert.DeserializeObject<EventSummary>( summaryString );
        }

        /// <summary>
        /// Converts the EventSummary into a JSON string
        /// </summary>
        /// <returns>The EventSummary object as a JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject( this );
            //return string.Format( @"{{""Count"":{0},""Open"":{1},""Exp"":{2},""AvgTime"":{3}}}", Count, Open, Expired, AverageTime );
        }
    }
}
