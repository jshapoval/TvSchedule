using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvSchedule.Data.Data;

namespace TvSchedule.Data.Tests
{
    [TestClass]
    public class SqlFilterTests
    {
        [TestClass]
        public class UnitTest1
        {
            [TestMethod]
            public void TestMethod_Eq()
            {
                var expected = "A = 5 AND B = 10 AND C = 7";
                var actual = DbHelper.GetWhereString(SqlFilter.Eq("A", 5), SqlFilter.Eq("B", 10), SqlFilter.Eq("C", 7));

                Assert.AreEqual(expected, actual);
            }


            [TestMethod]
            public void TestMethod_IsNull()
            {
                var expected = "A Is NULL";
                var actual = DbHelper.GetWhereString(SqlFilter.IsNull("A"));
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void TestMethod_IsNotNull()
            {
                var expected = "A Is NOT NULL";
                var actual = DbHelper.GetWhereString(SqlFilter.IsNotNull("A"));
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void TestMethod_Gt()
            {
                var expected = "6 > 5 AND 11 > 10 AND 8 > 7";
                var actual =
                    DbHelper.GetWhereString(SqlFilter.Gt("6", 5), SqlFilter.Gt("11", 10), SqlFilter.Gt("8", 7));

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void TestMethod_Lt()
            {
                var expected = "4 < 5 AND 9 < 10 AND 6 < 7";
                var actual = DbHelper.GetWhereString(SqlFilter.Lt("4", 5), SqlFilter.Lt("9", 10), SqlFilter.Lt("6", 7));

                Assert.AreEqual(expected, actual);
            }


            [TestMethod]
            public void TestMethod_In1()
            {
                var array = new[] { 2, 6, 7 };
                var str = "2, 6, 7";
                var expected = "A IN (2,6,7)";

                var actual = DbHelper.GetWhereString(SqlFilter.In("A", array)); //   Array.ForEach(array, i => )

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void TestMethod_In2()
            {
                var str = "2, 6, 7";
                var expected = "A IN (2,6,7)";
                var actual = DbHelper.GetWhereString(SqlFilter.In("A", 2, 6, 7)); //   Array.ForEach(array, i => )

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ChannelsDataContextTests_GetList()
            {
                var items = ChannelDataContext.Instance.GetList(2, SqlFilter.Eq("Name", "Ivan"));
                Assert.AreNotEqual(0, items.Count);
            }



            [TestMethod]
            public void ChannelsDataContextTests_GetOne()
            {
                var items = ChannelDataContext.Instance.GetOne(2);
                Assert.AreNotEqual(0, items);
            }


            [TestMethod]
            public void SqlFilterTests_Or()
            {
                var expected = "A=5 OR B=6 OR C=3";
                var actual =
                    DbHelper.GetWhereString(SqlFilter.Eq("A", 5).Or(SqlFilter.Eq("B", 6)).Or(SqlFilter.Eq("C", 3)));

                Assert.AreEqual(expected, actual);
                ;
            }


            [TestMethod]
            public void SqlDataContextBase_TryLock()
            {
                var channel = ChannelDataContext.Instance.GetOne(1);
                var result = ChannelDataContext.Instance.TryLock(channel, TimeSpan.FromHours(1),
                    ChannelDataContext.LockType.UpdateShows, "test1");
                Assert.AreEqual(result, true);
            }

            [TestMethod]
            public void SqlDataContextBase_TryRelease()
            {
                var channel = ChannelDataContext.Instance.GetOne(1);
                var result = ChannelDataContext.Instance.TryRelease(channel, "test1");
                Assert.AreEqual(result, true);
                result = ChannelDataContext.Instance.TryRelease(channel, "test2");
                Assert.AreEqual(result, false);
            }
        }
    }
}

