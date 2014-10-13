using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEventsSummary
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
            EventsSummary testSummary = new EventsSummary( new Dictionary<string, string>() { { "Test", "Test" } }, null );
            Assert.IsNotNull( testSummary.Details[ "Test" ] );
        }

        [TestMethod]
        public void DetailsIsCreatedWhenNull()
        {
            EventsSummary testSummary = new EventsSummary( null, null );
            Assert.IsNotNull( testSummary.Details );
        }

        [TestMethod]
        public void DetailsIsEmptyWhenNull()
        {
            EventsSummary testSummary = new EventsSummary( null, null );
            Assert.AreEqual( 0, testSummary.Details.Count );
        }

        [TestMethod]
        public void EventsIsSet()
        {
            EventsSummary testSummary = new EventsSummary( null, new Dictionary<string, Event>() { { "Test", new Event() } } );
            Assert.IsNotNull( testSummary.Events[ "Test" ] );
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
            Event testEvent = new Event();
            testEvent.Increment();
            EventsSummary testSummary = new EventsSummary( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, Event>() { { "Test", testEvent } } );
            Assert.IsFalse( string.IsNullOrEmpty( testSummary.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventOccurance()
        {
            Event testEvent = new Event();
            testEvent.Increment();
            EventsSummary expectedSummary = new EventsSummary( new Dictionary<string, string>() { { "Test", "Test" } }, new Dictionary<string, Event>() { { "Test", testEvent } } );
            EventsSummary testSummary = EventsSummary.FromString( expectedSummary.ToString() );
            Assert.AreEqual( expectedSummary.ToString(), testSummary.ToString() );
        }
    }
}
