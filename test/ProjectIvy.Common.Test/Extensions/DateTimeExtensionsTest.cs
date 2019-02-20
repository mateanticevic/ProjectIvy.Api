using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectIvy.Common.Extensions;
using System.Linq;
using System;

namespace ProjectIvy.Common.Test.Extensions
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

        [TestMethod]
        public void ConsecutiveDates_1()
        {
            var input = new[]
            {
                new DateTime(2018, 1, 4),
                new DateTime(2018, 1, 5),
                new DateTime(2018, 1, 6),
                new DateTime(2018, 1, 9),
                new DateTime(2018, 2, 1),
                new DateTime(2018, 2, 4),
                new DateTime(2018, 2, 7),
                new DateTime(2018, 2, 8)
            };

            var result = input.ConsecutiveDates().ToList();

            Assert.AreEqual(result[0].From, new DateTime(2018, 1, 4));
            Assert.AreEqual(result[0].To, new DateTime(2018, 1, 6));

            Assert.AreEqual(result[1].From, new DateTime(2018, 1, 9));
            Assert.AreEqual(result[1].To, new DateTime(2018, 1, 9));

            Assert.AreEqual(result[2].From, new DateTime(2018, 2, 1));
            Assert.AreEqual(result[2].To, new DateTime(2018, 2, 1));

            Assert.AreEqual(result[3].From, new DateTime(2018, 2, 4));
            Assert.AreEqual(result[3].To, new DateTime(2018, 2, 4));

            Assert.AreEqual(result[4].From, new DateTime(2018, 2, 7));
            Assert.AreEqual(result[4].To, new DateTime(2018, 2, 8));
        }
    }
}
