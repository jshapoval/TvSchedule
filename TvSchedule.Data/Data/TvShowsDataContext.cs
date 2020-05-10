using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TvSchedule.Data.Entities;


namespace TvSchedule.Data.Data
{
    public class TvShowsDataContext : SqlDataContextBase<string, TvShow>
    {
        public static TvShowsDataContext Instance = new TvShowsDataContext();

       
        public TvShowsDataContext() : base(new DbEntityInfo(new[] { new KeyColumn { Name = "Id", Type = DbType.String } }, false))
        {
        }
        protected override TvShow CreateValue(SqlDataReader reader)
        {
            return new TvShow { 
                Id = reader.Get<string>("id"), 
                Name = reader.Get<string>("Name"), 
                Description = reader.Get<string>("Description"), 
                ImageUrl = reader.Get<string>("ImageUrl"), 
                StartDateUtc = reader.Get<DateTime?>("StartDateUtc"), 
                ChannelId = reader.Get<int>("ChannelId"), 
                CreatedUtc = reader.Get<DateTime>("CreatedUtc"), 
                UpdatedUtc = reader.Get<DateTime?>("UpdatedUtc"), 
                IsDeleted = reader.Get<bool>("IsDeleted") };

        }

        protected override string TableName { get { return "TvShows"; } }

        //protected override int GetKey(TvShow value) { throw new NotImplementedException(); }

        public void Display(SqlDataReader reader)
        {
            Console.WriteLine($"Название: {reader["id"]}  Описание: {reader["Description"].ToString()}");
        }

        protected override Dictionary<string, object> GetValues(TvShow value)
        {
            return new Dictionary<string, object>
            {
                {"Id", value.Id},
                {"Name",value.Name },
                {"Description",value.Description },
                {"ImageUrl",value.ImageUrl },
                {"StartDateUtc",value.StartDateUtc },
                {"ChannelId",value.ChannelId },
                {"CreatedUtc",value.CreatedUtc },
                {"UpdatedUtc",value.UpdatedUtc },
                {"IsDeleted",value.IsDeleted },

            };
        }

    }

}
