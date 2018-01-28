using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectIvy.Common.Extensions;
using System.Linq;
using System;

namespace ProjectIvy.Tests.Common.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTest
    {
        [TestMethod]
        public void RangeMonthsClosed_1()
        {
            var from = new DateTime(2017,1,1);
            var to = new DateTime(2017,3,1);

            var range = from.RangeMonthsClosed(to)
                            .ToList();

            Assert.IsTrue(range.Count == 3);
            Assert.AreEqual(range[0].from, new DateTime(2017,1,1));
            Assert.AreEqual(range[0].to, new DateTime(2017,1,31));
            Assert.AreEqual(range[1].from, new DateTime(2017, 2, 1));
            Assert.AreEqual(range[1].to, new DateTime(2017, 2, 28));
            Assert.AreEqual(range[2].from, new DateTime(2017, 3, 1));
            Assert.AreEqual(range[2].to, new DateTime(2017, 3, 1));
        }

        [TestMethod]
        public void RangeMonthsClosed_2()
        {
            var from = new DateTime(2017, 12, 1);
            var to = new DateTime(2018, 1, 20);

            var range = from.RangeMonthsClosed(to)
                            .ToList();

            Assert.IsTrue(range.Count == 2);
            Assert.AreEqual(range[0].from, new DateTime(2017, 12, 1));
            Assert.AreEqual(range[0].to, new DateTime(2017, 12, 31));
            Assert.AreEqual(range[1].from, new DateTime(2018, 1, 1));
            Assert.AreEqual(range[1].to, new DateTime(2018, 1, 20));
        }
    }
}
