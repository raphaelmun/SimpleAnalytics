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
            Assert.AreEqual( Events.DefaultName, testEvent.Name );
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
        public void DetailAddsSingleValue()
        {
            Events testEvent = new Events();
            testEvent.Detail( "Test", "TestValue" );
            Assert.AreEqual( "TestValue", testEvent.Details[ "Test" ] );
        }

        [TestMethod]
        public void DetailReplacesSingleValueForExistingKey()
        {
            Events testEvent = new Events();
            testEvent.Detail( "Test", "TestValue" );
            testEvent.Detail( "Test", "TestNewValue" );
            Assert.AreEqual( "TestNewValue", testEvent.Details[ "Test" ] );
        }

        [TestMethod]
        public void DetailAddsMultipleValues()
        {
            Events testEvent = new Events();
            testEvent.Detail( new Dictionary<string, string>() { { "Test1", "1" }, { "Test2", "2" } } );
            Assert.AreEqual( "1", testEvent.Details[ "Test1" ] );
            Assert.AreEqual( "2", testEvent.Details[ "Test2" ] );
        }

        [TestMethod]
        public void DetailReplacesMultipleValues()
        {
            Events testEvent = new Events();
            testEvent.Detail( new Dictionary<string, string>() { { "Test1", "1" }, { "Test2", "2" } } );
            testEvent.Detail( new Dictionary<string, string>() { { "Test1", "New1" }, { "Test2", "New2" } } );
            Assert.AreEqual( "New1", testEvent.Details[ "Test1" ] );
            Assert.AreEqual( "New2", testEvent.Details[ "Test2" ] );
        }

        [TestMethod]
        public void DetailRetainsOldValues()
        {
            Events testEvent = new Events( new Dictionary<string, string>() { { "Test", "Test" } } );
            testEvent.Detail( new Dictionary<string, string>() { { "Test1", "1" }, { "Test2", "2" } } );
            Assert.AreEqual( "Test", testEvent.Details[ "Test" ] );
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
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent" );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Close( "TestEvent", id );
            Assert.AreEqual( 1, testEvent[ "TestEvent" ].Count );
        }

        [TestMethod]
        public void CloseUpdatesEventValue()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent" );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Close( "TestEvent", id );

            EventOccurance occurance = testEvent[ "TestEvent" ].Occurances.Last();
            Assert.IsTrue( occurance.FinishedTime > occurance.Time );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentException ) )]
        public void CloseFailsForNonexistentEventID()
        {
            Events testEvent = new Events();
            testEvent.Open( "TestEvent" );
            testEvent.Close( "TestEvent", Utility.GenerateUUID() );
        }

        [TestMethod]
        public void FlushExpiresOldOpenEvents()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent", 5 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();

            Assert.AreEqual( 1, testEvent[ "TestEvent" ].Count );
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
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Events testEvent = new Events();
            string id = testEvent.Open( "TestEvent", 5 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();

            Assert.AreEqual( 1, testEvent[ "TestEvent" ].ExpiredCount );
        }
    }
}
