using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Channels;
using  TvSchedule.Data.Entities;
using Channel = TvSchedule.Data.Entities.Channel;

namespace TvSchedule.Data.Data
{
    public class ChannelDataContext : SqlDataContextBase<int, Channel>
    {
        public static ChannelDataContext Instance = new ChannelDataContext();

        public ChannelDataContext() : base(
            new DbEntityInfo(
                new[] {new KeyColumn {Name = "Id", Type = DbType.Int32}},
                true,
                new[] {new KeyColumn {Name = "IdFromApi", Type = DbType.Int32}}))
        {
        }

        protected override Channel CreateValue(SqlDataReader reader)
        {
            return new Channel
            {
                Id = reader.Get<int>("Id"),
                Name = reader.Get<string>("Name"),
                Description = reader.Get<string>("Description"),
                CreatedUtc = reader.Get<DateTime>("CreatedUtc"),
                UpdateShowsLockSessionId = reader.Get<string>("UpdateShowsLockSessionId"),
                UpdateShowsLockExpireUtc = reader.Get<DateTime?>("UpdateShowsLockExpireUtc"),
                UpdatedUtc = reader.Get<DateTime?>("UpdatedUtc"),
                LockSessionId = reader.Get<string>("LockSessionId"),
                LockExpirationUtc = reader.Get<DateTime?>("LockExpirationUtc"),
                IdFromApi = reader.Get<int>("IdFromApi"),
                IsDeleted = reader.Get<bool>("IsDeleted"),
                LastError = reader.Get<string>("LastError"),
                LastShow = reader.Get<DateTime?>("LastShow"),
            };
        }


        protected override string TableName
        {
            get { return "Channels"; }
        }

        protected override int GetKey(Channel value)
        {
            return value.Id;
        }

        protected override Dictionary<string, object> GetValues(Channel value)
        {
            return new Dictionary<string, object>
            {
                {"Id", value.Id},
                {"Name", value.Name},
                {"Description", value.Description},
                {"CreatedUtc", value.CreatedUtc},
                {"UpdateShowsLockSessionId", value.UpdateShowsLockSessionId},
                {"UpdateShowsLockExpireUtc", value.UpdateShowsLockExpireUtc},
                {"UpdatedUtc", value.UpdatedUtc},
                {"LockSessionId", value.LockSessionId},
                {"LockExpirationUtc", value.LockExpirationUtc},
                {"IdFromApi", value.IdFromApi},
                {"IsDeleted", value.IsDeleted},
                {"LastError", value.LastError},
                {"LastShow", value.LastShow}
            };
        }

        public enum LockType
        {
            UpdateShows,
            UpdateChannelInfo
        }

        private (string lockSessionIdFieldName, string lockExpireFieldName) GetLockFields(LockType lockType)
        {
            switch (lockType)
            {
                case LockType.UpdateShows:
                    return (lockType + "LockSessionId", lockType + "LockExpireUtc");
                case LockType.UpdateChannelInfo:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException(lockType.ToString());
            }
        }

        public void Update(Channel value, Action<Channel> action, LockType lockType, string lockSessionId)
        {
            var fields = GetLockFields(lockType);

            UpdateProtected(
                value,
                action, 
                true,
                lockSessionId, 
                fields.lockSessionIdFieldName,
                fields.lockExpireFieldName);
        }

        public bool TryLock(Channel value, TimeSpan interval, LockType lockType, string lockSessionId )
        {
            var fields = GetLockFields(lockType);

            var res = TryLockProtected(
                value,
                interval,
                lockSessionId,
                fields.lockSessionIdFieldName,
                fields.lockExpireFieldName);
            if (res)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}