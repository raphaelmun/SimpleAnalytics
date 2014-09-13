using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEvent
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
        public void CountStartsAtZero()
        {
            Event testEvent = new Event();
            Assert.AreEqual( 0, testEvent.Count );
        }

        [TestMethod]
        public void OpenCountStartsAtZero()
        {
            Event testEvent = new Event();
            Assert.AreEqual( 0, testEvent.OpenCount );
        }

        [TestMethod]
        public void ExpiredCountStartsAtZero()
        {
            Event testEvent = new Event();
            Assert.AreEqual( 0, testEvent.ExpiredCount );
        }

        [TestMethod]
        public void IncrementAddsAnOccurance()
        {
            Event testEvent = new Event();
            testEvent.Increment();
            Assert.AreEqual( 1, testEvent.Count );
        }

        [TestMethod]
        public void OpenAddsAnOpenOccurance()
        {
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            Assert.AreEqual( 1, testEvent.OpenCount );
        }

        [TestMethod]
        public void OpenDoesNotAddToOccurances()
        {
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            Assert.AreEqual( 0, testEvent.Count );
        }

        [TestMethod]
        public void OpenOverwritesAnExistingOpenOccurance()
        {
            Event testEvent = new Event();
            string uuid = Utility.GenerateUUID();
            testEvent.Open( uuid, 10 );
            testEvent.Open( uuid, 10 );
            Assert.AreEqual( 1, testEvent.OpenCount );
        }

        [TestMethod]
        public void CloseRemovesOpenOccurance()
        {
            Event testEvent = new Event();
            string uuid = Utility.GenerateUUID();
            testEvent.Open( uuid, 10 );
            testEvent.Close( uuid );
            Assert.AreEqual( 0, testEvent.OpenCount );
        }

        [TestMethod]
        public void CloseAddsToOccurances()
        {
            Event testEvent = new Event();
            string uuid = Utility.GenerateUUID();
            testEvent.Open( uuid, 10 );
            testEvent.Close( uuid );
            Assert.AreEqual( 1, testEvent.Count );
        }

        [TestMethod]
        public void CloseSetsOccuranceValue()
        {
            Event testEvent = new Event();
            string uuid = Utility.GenerateUUID();
            testEvent.Open( uuid, 10 );
            testEvent.Close( uuid );
            EventOccurance occurance = testEvent.Occurances.Last();
            Assert.IsFalse( occurance.IsExpired );
        }

        [TestMethod]
        public void CloseReturnsFalseForNonexistentOpenOccurance()
        {
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            Assert.IsFalse( testEvent.Close( Utility.GenerateUUID() ) );
        }

        [TestMethod]
        public void FlushRemovesExpiredOccurances()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();
            Assert.AreEqual( 0, testEvent.OpenCount );
        }

        [TestMethod]
        public void FlushAddsToExpiredCountForExpiredOccurances()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();
            Assert.AreEqual( 1, testEvent.ExpiredCount );
        }

        [TestMethod]
        public void FlushAddsToOccurances()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();
            Assert.AreEqual( 1, testEvent.Count );
        }

        [TestMethod]
        public void FlushSetsOccuranceToExpired()
        {
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 0, 0 );
            Event testEvent = new Event();
            testEvent.Open( Utility.GenerateUUID(), 10 );
            SystemTime.UtcNowFunc = () => new DateTime( 2014, 9, 13, 0, 1, 0 );
            testEvent.Flush();
            EventOccurance occurance = testEvent.Occurances.Last();
            Assert.IsTrue( occurance.IsExpired );
        }
    }
}
