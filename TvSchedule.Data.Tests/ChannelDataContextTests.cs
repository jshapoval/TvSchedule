using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Channels;
using TvSchedule.Data.Data;
using TvSchedule.Data.Entities;
using Channel = TvSchedule.Data.Entities.Channel;

namespace TvSchedule.Data.Tests
{
    [TestClass]
    public class ChannelDataContextTests
    {
        [TestMethod]
        public void ChannelDataContextTests_GetList()
        {
            var items = ChannelDataContext.Instance.GetList(1, SqlFilter.Eq("Name", "Pervyy"), SqlFilter.IsNull("UpdatedUtc").Or(SqlFilter.Lt("UpdatedUtc", DateTime.UtcNow.AddHours(-4))));
            Assert.AreEqual(0, items.Count);
        }

        //[TestMethod]
        //public void TwShowDataContextTests_Merge()
        //{
        //    using (var bulk = TvShowsDataContext.Instance.StartBulkProcessing())
        //    {
        //        bulk.Merge(new TvShow(), x =>
        //        {
        //            x.Id = TvShow.GetHash(x.Name = "razgovorysbatushkoy", x.ChannelId = 3, x.StartDateUtc = DateTime.MaxValue);

        //            x.Description = "polezno";
        //        });

        //        bulk.Merge(new TvShow(), x =>
        //        {
        //            x.Id = TvShow.GetHash(x.Name = "novosti", x.ChannelId = 4, x.StartDateUtc = DateTime?.MaxValue);

        //            x.Description = "pravdivo";
        //        });

        //        bulk.Merge(new TvShow(), x =>
        //        {
        //            x.Id = TvShow.GetHash(x.Name = "beremenav16", x.ChannelId = 5, x.StartDateUtc = DateTime.MaxValue);

        //            x.Description = "grustno";
        //        });

        //    }
        //}

        [TestMethod]
        public void ChannelDataContextTests_Merge()
         {
            using (var bulk = ChannelDataContext.Instance.StartBulkProcessing())
            {
                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Description = "First";
                //    x.Name = "1";
                //});

                //bulk.Merge(new Channel(), x =>
                //{
                //    //x.Id = 4;
                //    x.Description = "second";
                //});
                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Description = "1jjjjjjj03";
                //});

                bulk.Merge(new Channel(), x =>
                {
                    x.Description = "20";
                });


                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Id = 2014;
                //    x.Description = "11111111903";
                //});

                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Id = 2015;
                //    x.Description = "1903";
                //});

                //bulk.Merge(new Channel(), x =>
                //{
                //    // x.Id = 14;
                //    x.Description = "nnnnnn";
                //});

                //bulk.Merge(new Channel(), x =>
                //{
                //    x.Id = 3;
                //    x.Description = "F";
                //    x.Name = "F";
                //});

                //var channel2 = new Channel();
                //bulk.Merge(channel2, x =>
                //{
                //    x.Id = 1;
                //    x.Name = "newTest";
                //    x.Description = "teeeest";
                //});

                //var channel4 = new Channel();
                //bulk.Merge(channel4, x =>
                //{
                //    x.Id = 3;
                //    x.Name = "FC";
                //});

                ////var channel5 = new Channel();
                ////bulk.Merge(channel5, x =>
                ////{
                ////    x.Id = 4;
                ////    x.Name = "FC";
                ////    x.Description = "DFC";
                ////});
            }
        }



        [TestMethod]
        public void ViewerDataContextTests_MergeForViewer()
        {
            using (var bulk = ViewerDataContext.Instance.StartBulkProcessing())
            {
                //var viewer = new Viewer();
                //bulk.Merge(viewer, x =>
                //{
                //    x.Id = 1;
                //    x.Name = "TIvan";
                //    x.Description = "Ttest";
                //    x.DescriptionForTest = "ONE";//});

                var viewer2 = new Viewer();
                bulk.Merge(viewer2, x =>
                {
                    x.Id = 1; x.Name = "Ivan";
                    x.Description = "test";
                    x.DescriptionForTest = "1903";
                });

                var viewer3 = new Viewer();
                bulk.Merge(viewer3, x =>
                {
                    x.Id = 1;
                    x.Name = "1";
                    x.Description = "1";
                    x.DescriptionForTest = "1903";
                });

                var viewer4 = new Viewer();
                bulk.Merge(viewer4, x =>
                {
                    x.Id = 9; x.Name = "9";
                    x.Description = "9";
                    x.DescriptionForTest = "1903";
                });

                //var viewer5 = new Viewer();
                //bulk.Merge(viewer5, x =>
                //{
                //    x.Id = 3;
                //    x.Name = "TSpas";
                //    x.Description = "Tpravoslavnaaaaa";
                //    x.DescriptionForTest = "THird";
                //});


                //var viewer4 = new Viewer();
                //bulk.Merge(viewer4, x =>
                //{
                //    x.Id = 3;
                //    x.Name = "Spas";
                //    x.DescriptionForTest = "break";
                //});
            }

        }



        [TestMethod]
        public void ChannelDataContextTests_Commit()
        {
            using (var bulk = ChannelDataContext.Instance.StartBulkProcessing())
            {
                DataTable _dataTable = new DataTable();
                _dataTable.Clear();
                _dataTable.PrimaryKey = new DataColumn[] { _dataTable.Columns.Add("Id", typeof(Int32)) };
                _dataTable.Columns.Add("Name", typeof(String));
                _dataTable.Columns.Add("Age", typeof(Int32));
                object[] ob1 = { 1, "Masha", 16 };
                object[] ob2 = { 2, "Misha", 13 };
                _dataTable.Rows.Add(ob1);
                _dataTable.Rows.Add(ob2);
            }
        }

    }
}
