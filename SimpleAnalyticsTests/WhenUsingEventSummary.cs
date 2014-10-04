using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEventSummary
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
        public void CountIsSet()
        {
            const int expectedValue = 12345;
            EventSummary testSummary = new EventSummary( expectedValue, 0, 0, 1.0f );
            Assert.AreEqual( expectedValue, testSummary.Count );
        }

        [TestMethod]
        public void OpenIsSet()
        {
            const int expectedValue = 12345;
            EventSummary testSummary = new EventSummary( 0, expectedValue, 0, 1.0f );
            Assert.AreEqual( expectedValue, testSummary.Open );
        }

        [TestMethod]
        public void ExpiredIsSet()
        {
            const int expectedValue = 12345;
            EventSummary testSummary = new EventSummary( 0, 0, expectedValue, 1.0f );
            Assert.AreEqual( expectedValue, testSummary.Expired );
        }

        [TestMethod]
        public void AverageTimeIsSet()
        {
            const float expectedValue = 1.2345f;
            EventSummary testSummary = new EventSummary( 0, 0, 0, expectedValue );
            Assert.AreEqual( expectedValue, testSummary.AverageTime );
        }

        [TestMethod]
        public void ToStringIsNotEmpty()
        {
            EventSummary testSummary = new EventSummary( 12345, 12345, 12345, 1.2345f );
            Assert.IsFalse( string.IsNullOrEmpty( testSummary.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventOccurance()
        {
            EventSummary expectedSummary = new EventSummary( 12345, 12345, 12345, 1.2345f );
            EventSummary testSummary = EventSummary.FromString( expectedSummary.ToString() );
            Assert.AreEqual( expectedSummary.ToString(), testSummary.ToString() );
        }
    }
}
