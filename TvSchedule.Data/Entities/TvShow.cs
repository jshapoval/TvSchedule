using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using TvSchedule.Data.Data;

namespace TvSchedule.Data.Entities
{
    public class TvShow : IDbEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public int ChannelId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
        public bool IsDeleted { get; set; }

        public static string GetHash(string name, int channelId, DateTime startDate)
        {
            string channelName = "";
            using (SqlConnection connection = new SqlConnection(TvShowsDataContext.CONNECTION_STRING))
            {
                connection.Open();
                var selectChannelName = string.Format(
                    @"SELECT TOP 1 Name FROM Channels WHERE Id = {0}", channelId
                );

                SqlCommand selectCommand = new SqlCommand(selectChannelName, connection);
                channelName = selectCommand.ExecuteScalar().ToString();
            }

            var key = string.Concat(channelName, name, startDate);

            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(key);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
