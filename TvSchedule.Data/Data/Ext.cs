using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TvSchedule.Data.Data
{
    public static class Ext
    {
        public static string GetDescription(this Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()?.Description
                ?? value.ToString();
        }

        public static T Get<T>(this SqlDataReader reader, string name)
        {
            var value = reader[name];

            var t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return value == null || DBNull.Value.Equals(value) ?
                default : (T)Convert.ChangeType(value, t);
        }
    }
}
