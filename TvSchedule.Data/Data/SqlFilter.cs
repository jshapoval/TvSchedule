using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TvSchedule.Data.Data
{
    public class SqlFilter
    {
        enum ConditionType
        {
            [Description("=")]
            Eq,
            [Description("<>")]
            Ne,
            [Description("OR")]
            OR,
            [Description("AND")]
            AND,
            [Description("IN")]
            In,
            [Description(">")]
            Gt,
            [Description("<")]
            Lt,
            [Description("Is NULL")]
            IsNull,
            [Description("Is NOT NULL")]
            IsNotNull
        }

        private ConditionType Type { get; set; }
        public string Column { get; private set; }
        private object Value { get; set; }
        private SqlFilter[] InnerFilters { get; set; }

        public static SqlFilter Eq(string column, object value)
        {
            return new SqlFilter { Type = ConditionType.Eq, Column = column, Value = value };
        }

     

        public static SqlFilter Gt(string column, object value)
        {
            return new SqlFilter { Type = ConditionType.Gt, Column = column, Value = value };
        }
        public SqlFilter Or(params SqlFilter[] filters)
        {
            return new SqlFilter { InnerFilters = new[] { this }.Concat(filters).ToArray(), Type = ConditionType.OR };
        }

        public static SqlFilter IsNull(string column)
        {
            return new SqlFilter { Type = ConditionType.IsNull, Column = column };
        }

        public static SqlFilter IsNotNull(string column)
        {
            return new SqlFilter { Type = ConditionType.IsNotNull, Column = column };
        }

        public static SqlFilter In(string column, params object[] values)
        {
            return new SqlFilter { Type = ConditionType.In, Column = column, Value = values };
        }

        public static SqlFilter In(string column, object values)
        {
            return new SqlFilter { Type = ConditionType.In, Column = column, Value = values };
        }

        public static SqlFilter Lt(string column, object value)
        {
            return new SqlFilter { Type = ConditionType.Lt, Column = column, Value = value};
        }


        private static string AddQuotesIfString(object value)
        {
            //return value is string ? string.Concat("'", value,"'") : value.ToString();

            switch (value)
            {
                case string stringValue:
                    return string.Concat("'", stringValue, "'");
                case DateTime dateTimeValue:
                    return string.Concat("'", dateTimeValue.ToString("yyyy-MM-dd hh:mm:ss"), "'"); ;
                default:
                    return value?.ToString();
            }
        }

        private static string ProcessValue(object value)
        {
            var collection = value as ICollection;

            if (collection == null)
            {
                 return AddQuotesIfString(value);
            }

            string result = null;

            foreach (var item in collection)
            {
                if (result != null)
                {
                    result += ",";
                }

                result += AddQuotesIfString(item);
            }

            return string.Concat("(", result, ")");
        }

        public override string ToString()
        {
            if (InnerFilters == null)
            {
                return string.Concat(Column,
                    " ",
                    Type.GetDescription(),
                    " ",
                    ProcessValue(Value)
                ).Trim();
            }
            return string.Join<SqlFilter>(string.Concat(" ", Type.ToString(), " "), InnerFilters);
        }

    }
}
