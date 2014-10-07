using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEventsHistory
    {
        [TestInitialize]
        public void Setup()
        {
            SystemTime.ResetToDefault();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SystemTime.ResetToDefault();
        }

        [TestMethod]
        public void DetailsIsSet()
        {
            EventsHistory testHistory = new EventsHistory( new Dictionary<string, string>() { { "Test", "Test" } }, null );
            Assert.IsNotNull( testHistory.Details[ "Test" ] );
        }

        [TestMethod]
        public void DetailsIsCreatedWhenNull()
        {
            EventsHistory testHistory = new EventsHistory( null, null );
            Assert.IsNotNull( testHistory.Details );
        }

        [TestMethod]
        public void DetailsIsEmptyWhenNull()
        {
            EventsHistory testHistory = new EventsHistory( null, null );
            Assert.AreEqual( 0, testHistory.Details.Count );
        }

        [TestMethod]
        public void EventsIsSet()
        {
            EventsHistory testHistory = new EventsHistory( null, new Dictionary<string, EventsSummaryDataPoint[]>() { { "Test", new EventsSummaryDataPoint[] { new EventsSummaryDataPoint( SystemTime.UtcNow, new EventSummary( 12, 34, 56, 7.8f ) ) } } } );
            Assert.IsNotNull( testHistory.Events[ "Test" ] );
        }

        [TestMethod]
        public void EventsIsCreatedWhenNull()
        {
            EventsHistory testHistory = new EventsHistory( null, null );
            Assert.IsNotNull( testHistory.Events );
        }

        [TestMethod]
        public void EventsIsEmptyWhenNull()
        {
            EventsHistory testHistory = new EventsHistory( null, null );
            Assert.AreEqual( 0, testHistory.Events.Count );
        }

        [TestMethod]
        public void ToStringIsNotEmpty()
        {
            EventsHistory testHistory = new EventsHistory( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, EventsSummaryDataPoint[]>() { { "Test", new EventsSummaryDataPoint[] { new EventsSummaryDataPoint( SystemTime.UtcNow, new EventSummary( 12, 34, 56, 7.8f ) ) } } } );
            Assert.IsFalse( string.IsNullOrEmpty( testHistory.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventOccurance()
        {
            EventsHistory expectedHistory = new EventsHistory( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, EventsSummaryDataPoint[]>() { { "Test", new EventsSummaryDataPoint[] { new EventsSummaryDataPoint( SystemTime.UtcNow, new EventSummary( 12, 34, 56, 7.8f ) ) } } } );
            EventsHistory testHistory = EventsHistory.FromString( expectedHistory.ToString() );
            Assert.AreEqual( expectedHistory.ToString(), testHistory.ToString() );
        }
    }
}
