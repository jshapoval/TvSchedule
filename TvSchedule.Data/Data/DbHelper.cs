using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TvSchedule.Data.Data
{
    public static class DbHelper
    {
        public static string GetWhereString(params SqlFilter[] filters)
        {
            return string.Join<SqlFilter>(" AND ", filters);
        }


        public static string GetSelectRowByPrimaryKey(DbEntityInfo entityInfo, params SqlFilter[] filters)
        {
            List<SqlFilter> filterList = new List<SqlFilter>(3);
            foreach (var k in entityInfo.PrimaryKeyColumns)
            {
                var filter = filters.First(f => f.Column.Equals(k.Name, StringComparison.OrdinalIgnoreCase));
                filterList.Add(filter);
            }
            return GetWhereString(filterList.ToArray());
        }
    }
}
