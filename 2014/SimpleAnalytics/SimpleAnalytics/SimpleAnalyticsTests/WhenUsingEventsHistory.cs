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
            EventsHistory testHistory = new EventsHistory( null, new Dictionary<string, EventsSummaryDataPoint[]>() { { "Test", new EventsSummaryDataPoint[] { new EventsSummaryDataPoint( SystemTime.UtcNow, new EventsSummary( null, null ) ) } } } );
            Assert.IsNotNull( testHistory.Events[ "Test" ] );
        }

        [TestMethod]
        public void EventsIsCreatedWhenNull()
        {
            EventsSummary testSummary = new EventsSummary( null, null );
            Assert.IsNotNull( testSummary.Events );
        }

        [TestMethod]
        public void EventsIsEmptyWhenNull()
        {
            EventsSummary testSummary = new EventsSummary( null, null );
            Assert.AreEqual( 0, testSummary.Events.Count );
        }

        [TestMethod]
        public void ToStringIsNotEmpty()
        {
            EventsSummary testSummary = new EventsSummary( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, Event>() { { "Test", new Event() } } );
            Assert.IsFalse( string.IsNullOrEmpty( testSummary.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventOccurance()
        {
            EventsSummary expectedSummary = new EventsSummary( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, Event>() { { "Test", new Event() } } );
            EventsSummary testSummary = EventsSummary.FromString( expectedSummary.ToString() );
            Assert.AreEqual( expectedSummary.ToString(), testSummary.ToString() );
        }
    }
}
