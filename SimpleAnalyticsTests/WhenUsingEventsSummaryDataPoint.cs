using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEventsSummaryDataPoint
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
        public void TimeIsSet()
        {
            DateTime expectedTime = SystemTime.UtcNow;
            EventsSummaryDataPoint testDataPoint = new EventsSummaryDataPoint( expectedTime, new EventSummary( 0, 0, 0, 0 ) );
            Assert.AreEqual( expectedTime, testDataPoint.Time );
        }

        [TestMethod]
        public void SummaryIsSet()
        {
            EventSummary expectedSummary = new EventSummary( 12, 34, 56, 7.8f );
            EventsSummaryDataPoint testDataPoint = new EventsSummaryDataPoint( SystemTime.UtcNow, expectedSummary );
            Assert.AreEqual( expectedSummary, testDataPoint.Summary );
        }

        [TestMethod]
        public void ToStringIsNotEmpty()
        {
            EventsSummaryDataPoint testDataPoint = new EventsSummaryDataPoint( SystemTime.UtcNow, new EventSummary( 12, 34, 56, 7.8f ) );
            Assert.IsFalse( string.IsNullOrEmpty( testDataPoint.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventsSummaryDataPoint()
        {
            EventsSummaryDataPoint expectedDataPoint = new EventsSummaryDataPoint( SystemTime.UtcNow, new EventSummary( 12, 34, 56, 7.8f ) );
            EventsSummaryDataPoint testDataPoint = EventsSummaryDataPoint.FromString( expectedDataPoint.ToString() );
            Assert.AreEqual( expectedDataPoint.ToString(), testDataPoint.ToString() );
        }
    }
}
