using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAnalytics;

namespace SimpleAnalyticsTests
{
    [TestClass]
    public class WhenUsingEventOccurance
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
        public void StartTimeIsSet()
        {
            DateTime expectedTime = SystemTime.UtcNow;
            EventOccurance testEo = new EventOccurance( expectedTime, DateTime.MaxValue, DateTime.MaxValue );
            Assert.AreEqual( expectedTime, testEo.Start );
        }

        [TestMethod]
        public void FinishedTimeIsSet()
        {
            DateTime expectedTime = SystemTime.UtcNow;
            EventOccurance testEo = new EventOccurance( DateTime.MaxValue, expectedTime, DateTime.MaxValue );
            Assert.AreEqual( expectedTime, testEo.Finished );
        }

        [TestMethod]
        public void ExpirationIsSet()
        {
            DateTime expectedTime = SystemTime.UtcNow;
            EventOccurance testEo = new EventOccurance( DateTime.MaxValue, DateTime.MaxValue, expectedTime );
            Assert.AreEqual( expectedTime, testEo.Expiration );
        }

        [TestMethod]
        public void ToStringIsNotEmpty()
        {
            EventOccurance testEo = new EventOccurance( SystemTime.UtcNow, SystemTime.UtcNow, SystemTime.UtcNow );
            Assert.IsFalse( string.IsNullOrEmpty( testEo.ToString() ) );
        }

        [TestMethod]
        public void FromStringCreatesEqualEventOccurance()
        {
            EventOccurance expectedEo = new EventOccurance( SystemTime.UtcNow, SystemTime.UtcNow, SystemTime.UtcNow );
            EventOccurance testEo = EventOccurance.FromString( expectedEo.ToString() );
            Assert.AreEqual( expectedEo.ToString(), testEo.ToString() );
        }
    }
}
