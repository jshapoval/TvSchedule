using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Channels;
using TvSchedule.Data.Data;
using TvSchedule.Data.Entities;
using Channel = TvSchedule.Data.Entities.Channel;

namespace TvSchedule.Data.Tests
{
    [TestClass]
    public class AddOrUpdateTests
    {
        [TestMethod]
        public void AddOrUpdateTests_Merge_SinglePrimaryKey()
        {
            using (var bulk = ChannelDataContext.Instance.StartBulkProcessing())
            {
                bulk.Merge(new Channel(), x =>
                {
                    x.Name = "First";
                    x.Description = "1";
                    x.IdFromApi = 11;
                });

                bulk.Merge(new Channel(), x =>
                {
                    x.Description = "2";
                    x.IdFromApi = 12;
                });

                bulk.Merge(new Channel(), x =>
                {
                    x.Name = "Third";
                    x.Description = "3";
                    x.IdFromApi = 13;
                });

                bulk.Merge(new Channel(), x =>
                {
                    x.Description = "4";
                    x.IdFromApi = 14;
                });

                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Name = "Third_UpdateByMerge";
                //});
            }
        }

        [TestMethod]
        public void AddOrUpdateTests_Update_SinglePrimaryKey()
        {
            using (var bulk = ChannelDataContext.Instance.StartBulkProcessing())
            {
                //bulk.Update(new Channel(), x =>
                //{
                //    x.Id = 1;
                //    x.Name = "Pervyy";
                //    x.Description = "Patriotichna";
                //});

                //bulk.Update(new Channel(), x =>
                //{
                //    x.Id = 2;
                //    x.Name = "spas";
                //});
            }
        }

        [TestMethod]
        public void AddOrUpdateTests_Merge_ComplexPrimaryKey()
        {
            using (var bulk = ViewerDataContext.Instance.StartBulkProcessing())
            {
                bulk.Merge(new Viewer(), x =>
                {
                    x.Id = 1;
                    x.Name = "First";
                    x.Description = "1";

                    x.DescriptionForTest = "FirstDescription";
                });

                bulk.Merge(new Viewer(), x =>
                {
                    x.Id = 1;
                    x.Name = "Second";
                    x.Description = "2";

                    x.DescriptionForTest = "SecondDescription";
                });

                bulk.Merge(new Viewer(), x =>
                {
                    x.Id = 1;
                    x.Name = "First";
                    x.Description = "1";

                    x.DescriptionForTest = "FirstDescriptionByMerge";
                });
            }
        }

        [TestMethod]
            public void AddOrUpdateTests_Update_ComplexPrimaryKey()
            {
                using (var bulk = ViewerDataContext.Instance.StartBulkProcessing())
                {
                    bulk.Merge(new Viewer(), x =>
                    {
                        x.Id = 1;
                        x.Name = "First";
                        x.Description = "1";

                        x.DescriptionForTest = "FirstDescriptionAfterUpdate";
                    });
                }
            }



        [TestMethod]
        public void AddOrUpdateTests_MergeViever_ComplexPK()
        {
            var expectedString = "thisDescriptionAfterUpdate";
            using (var bulk = ViewerDataContext.Instance.StartBulkProcessing())
            {
                bulk.Merge(new Viewer(), x =>
                {
                    x.Id = 1;
                    x.Name = "First";
                    x.Description = "1";

                    x.DescriptionForTest = expectedString;
                });
            }
            string actual = "";


            using (SqlConnection connection = new SqlConnection(ViewerDataContext.CONNECTION_STRING))
            {
                connection.Open();
                var selectChannelName = string.Format(
                    @"SELECT TOP 1 DescriptionForTest FROM Viewers WHERE Id = 1 AND Name = 'First' AND Description = '1'"
                );

                SqlCommand selectCommand = new SqlCommand(selectChannelName, connection);
                actual = selectCommand.ExecuteScalar().ToString();
            }

            //var items = ChannelDataContext.Instance.GetOne(3007);
            Assert.AreEqual(expectedString, actual);
        }

        //[TestMethod]
        //    public void AddOrUpdateTests_Update_ComplexPK()
        //    {
        //        var items = ViewerDataContext.Instance.GetOne(1,"First", "1");
        //        Assert.AreEqual(1, items);
        //    }

        //[TestMethod]
        //public void AddOrUpdateTests_Merge_SinglePK()
        //{
        //    var items = ChannelDataContext.Instance.GetList(1, SqlFilter.Eq("Name", "Pervyy"), SqlFilter.IsNull("UpdatedUtc").Or(SqlFilter.Lt("UpdatedUtc", DateTime.UtcNow.AddHours(-4))));
        //    Assert.AreEqual(0, items.Count);
        //}

    }
}
