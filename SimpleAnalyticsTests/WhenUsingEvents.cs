using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEvents
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
        public void ConstructorSetsDefaultName()
        {
            Events testEvent = new Events();
            Assert.AreEqual( "Default", testEvent.Name );
        }

        [TestMethod]
        public void ConstructorSetsDetails()
        {
            Events testEvent = new Events( new Dictionary<string, string>() { { "Test", "Test" } } );
            Assert.AreEqual( "Test", testEvent.Details[ "Test" ] );
        }

        [TestMethod]
        public void ConstructorSetsNameFromDetails()
        {
            Events testEvent = new Events( new Dictionary<string, string>() { { "Name", "Test" } } );
            Assert.AreEqual( "Test", testEvent.Name );
        }

        [TestMethod]
        public void ConstructorSetsSessionID()
        {
            Events testEvent = new Events();
            Assert.IsFalse( string.IsNullOrEmpty( testEvent.SessionID ) );
        }

        [TestMethod]
        public void NameSetsName()
        {
            Events testEvent = new Events();
            testEvent.Name = "Test";
            Assert.AreEqual( "Test", testEvent.Name );
        }

        [TestMethod]
        public void NameInDetailSetsName()
        {
            Events testEvent = new Events();
            testEvent.Detail( new Dictionary<string, string>() { { "Name", "Test" } } );
            Assert.AreEqual( "Test", testEvent.Name );
        }

        [TestMethod]
        public void IncrementCreatesNewEvent()
        {
            Events testEvent = new Events();
            testEvent.Increment( "TestEvent" );
            Assert.IsNotNull( testEvent[ "TestEvent" ] );
        }

        [TestMethod]
        public void IncrementAddsCountToEvent()
        {
            Events testEvent = new Events();
            testEvent.Increment( "TestEvent" );
            Assert.AreEqual( 1, testEvent[ "TestEvent" ].Count );
        }

        [TestMethod]
        public void OpenCreatesNewEvent()
        {
            Events testEvent = new Events();
            testEvent.Open( "TestEvent" );
            Assert.IsNotNull( testEvent[ "TestEvent" ] );
        }

        [TestMethod]
        public void CloseAddsToTheListOfOccurances()
        {
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent" );
            SystemTime.UtcNowFunc = () => DateTime.UtcNow + TimeSpan.FromSeconds( 5 );
            testEvent.Close( id );
            Assert.AreEqual( 1, testEvent[ "TestEvent" ].Count );
        }

        [TestMethod]
        public void CloseUpdatesEventValue()
        {
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent" );
            SystemTime.UtcNowFunc = () => DateTime.UtcNow + TimeSpan.FromSeconds( 5 );
            testEvent.Close( id );

            EventOccurance occurance = testEvent[ "TestEvent" ].Occurances.Last();
            Assert.IsTrue( occurance.FinishedTime > occurance.Time );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentException ) )]
        public void CloseFailsForNonexistentEventID()
        {
            Events testEvent = new Events();
            testEvent.Close( Utility.GenerateUUID() );
        }

        [TestMethod]
        public void FlushExpiresOldOpenEvents()
        {
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent", 5 );
            SystemTime.UtcNowFunc = () => DateTime.UtcNow + TimeSpan.FromSeconds( 10 );
            testEvent.Flush();

            Assert.AreEqual( 0, testEvent[ "TestEvent" ].Count );
            Assert.AreEqual( 0, testEvent[ "TestEvent" ].OpenCount );
        }

        [TestMethod]
        public void NonexistentEventNameReturnsNull()
        {
            Events testEvent = new Events();
            Assert.IsNull( testEvent[ "TestEvent" ] );
        }

        [TestMethod]
        public void ExpiredEventsAreAddedToExpiredCount()
        {
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent", 5 );
            SystemTime.UtcNowFunc = () => DateTime.UtcNow + TimeSpan.FromSeconds( 10 );
            testEvent.Flush();

            Assert.AreEqual( 1, testEvent[ "TestEvent" ].ExpiredCount );
        }
    }
}
