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
    public class CRUDTests
    {
        [TestMethod]
        public void CRUDTests_Create_SinglePrimaryKey()
        {
            ChannelDataContext.Instance.Create(new Channel(), x =>
            {
                    x.Name = "090420";
                    x.Description = "11111111";
                    x.IdFromApi = 20;
                    x.IsDeleted = false;
            });

            //ChannelDataContext.Instance.Create(new Channel(), x =>
            //{
            //    x.Name = "090420";
            //    x.Description = "090420";
            //    x.IdFromApi = 19;
            //});
        }

        [TestMethod]
        public void CRUDTests_Update_SinglePrimaryKey()
        {
            ChannelDataContext.Instance.Update(new Channel(), x =>
            {
                x.Id = 1008;
                x.UpdateShowsLockExpireUtc = DateTime.MaxValue;
                x.IsDeleted = false;
            });

            ChannelDataContext.Instance.Update(new Channel(), x =>
            {
                x.Id = 1010;
                x.UpdateShowsLockExpireUtc = DateTime.MaxValue;
                x.IsDeleted = true;
            });
        }

        [TestMethod]
        public void CRUDTests_Delete_SinglePrimaryKey()
        {
            ChannelDataContext.Instance.Delete(new Channel() { Id = 1008, UpdateShowsLockExpireUtc = DateTime.MaxValue});
        }

        [TestMethod]
        public void CRUDTests_Create_ComplexPrimaryKey()
        {
            ViewerDataContext.Instance.Create(new Viewer(), x =>
            {
                x.Id = 2;
                x.Name = "09999990420";
                x.Description = "1111";

                x.DescriptionForTest = "test";
            });

            ViewerDataContext.Instance.Create(new Viewer(), x =>
            {
                x.Id = 2;
                x.Name = "090420";
                x.Description = "0999990420";

                x.DescriptionForTest = "090jhv420";
            });
        }

        [TestMethod]
            public void CRUDTests_Update_ComplexPrimaryKey()
            {
            ViewerDataContext.Instance.Update(new Viewer(), x =>
            {
                x.Id = 2;
                x.Name = "080420";
                x.Description = "1111";

                x.DescriptionForTest = "TEST";
            });

            ViewerDataContext.Instance.Update(new Viewer(), x =>
            {
                x.Id = 2;
                x.Name = "080420";
                x.Description = "080420";

                x.DescriptionForTest = "TEST";
            });
        }

            [TestMethod]
            public void CRUDTests_Delete_ComplexPrimaryKey()
            {
                ViewerDataContext.Instance.Delete(new Viewer() { Id = 2, Name = "090420", Description = "1111", UpdateShowsLockExpireUtc = DateTime.MaxValue });
        }


            [TestMethod]
        public void CRUDTests_CheckCreate_ComplexPrimaryKey()
        {
            var expectedString = "thisDescriptionAfterUpdate";
            ViewerDataContext.Instance.Create(new Viewer(), x =>
            {
                x.Id = 1;
                x.Name = "00000";
                x.Description = "11111";

                x.DescriptionForTest = expectedString;
            });

            string actual = "";
            using (SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TvSchedule;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                connection.Open();
                var selectChannelName = string.Format(
                    @"SELECT TOP 1 DescriptionForTest FROM Viewers WHERE Id = 1 AND Name = '00000' AND Description = '11111'"
                );

                SqlCommand selectCommand = new SqlCommand(selectChannelName, connection);
                actual = selectCommand.ExecuteScalar().ToString();
            }

            Assert.AreEqual(expectedString, actual);
        }



        [TestMethod]
        public void CRUDTests_Update_Channels()
        {
            var expectedString = "thisDescriptionAfterUpdate";
            ChannelDataContext.Instance.Update(new Channel(), x =>
            {
                x.Id = 4332;
                x.Description = "thisDescriptionAfterUpdate";

            });

            string actual = "";
            using (SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TvSchedule;User Id=Julia;Password=a72a48a1000225;Integrated Security=False"))
            {
                connection.Open();
                var selectChannelName = string.Format(
                    @"SELECT TOP 1 Description FROM Channels WHERE Id = 4332"
                );

                SqlCommand selectCommand = new SqlCommand(selectChannelName, connection);
                actual = selectCommand.ExecuteScalar().ToString();
            }

            Assert.AreEqual(expectedString, actual);
        }
    }
}
