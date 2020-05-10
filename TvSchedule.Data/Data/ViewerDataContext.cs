using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Channels;
using  TvSchedule.Data.Entities;

namespace TvSchedule.Data.Data
{
    public class ViewerDataContext : SqlDataContextBase<(int, string, string), Viewer>
    {
        public static ViewerDataContext Instance = new ViewerDataContext();

        public ViewerDataContext() : base(new DbEntityInfo(new[] { new KeyColumn { Name = "Id", Type = DbType.Int32}, new KeyColumn { Name = "Name", Type = DbType.String }, new KeyColumn { Name = "Description", Type = DbType.String } }, false))
        {
        }
        protected override Viewer CreateValue(SqlDataReader reader)
        {
            return new Viewer()
            {
                Id = (int) reader["id"], Name = (string) reader["Name"], DOB = (DateTime) reader["DOB"], Description = (string) reader["Description"],
                DescriptionForTest = (string)reader["DescriptionForTest"],
                CreatedUtc = (DateTime) reader["CreatedUtc"],
                UpdateShowsLockSessionId = (string) reader["UpdateShowsLockSessionId"],
                UpdateShowsLockExpireUtc = (DateTime) reader["UpdateShowsLockExpireUtc"],
                UpdatedUtc = (DateTime) reader["UpdatedUtc"], LockSessionId = (string) reader["LockSessionId"],
                LockExpirationUtc = (DateTime?) reader["LockExpirationUtc"]
            };

        }

        public Viewer GetOne(int id, string name, string description)
        {
            var query = $"select top 1 * from {TableName} where Id = {id} and Name = '{name}' and Description = '{description}'";

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            return CreateValue(reader);
                        }
                    }
                }
            }
            return default(Viewer);
        }


        protected override string TableName
        { 
            get { return "Viewers"; }
        }

        protected override (int,string,string) GetKey(Viewer value)
        {
            var result = (id: 0, name:" ", desc: " ");
            result.id = value.Id;
            result.name = value.Name;
            result.desc = value.Description;
            return result;
        }

        protected override Dictionary<string, object> GetValues(Viewer value)
        {
            return new Dictionary<string, object>
            {
                {"Id", value.Id},
                {"Name",value.Name },
                {"DOB", value.DOB },
                {"Description",value.Description },
                {"DescriptionForTest",value.DescriptionForTest },
                {"CreatedUtc",value.CreatedUtc },
                {"UpdateShowsLockSessionId", value.UpdateShowsLockSessionId},
                {"UpdateShowsLockExpireUtc",value.UpdateShowsLockExpireUtc },
                {"UpdatedUtc",value.UpdatedUtc },
                {"LockSessionId",value.LockSessionId },
                {"LockExpirationUtc",value.LockExpirationUtc },

            };
        }
    }

}