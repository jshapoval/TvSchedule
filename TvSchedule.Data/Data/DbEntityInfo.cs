using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TvSchedule.Data.Data
{
    public class DbEntityInfo
    {
        public KeyColumn[] PrimaryKeyColumns { get; set; }
        public KeyColumn[][] UniqueConstraints { get; set; }
        public bool Autoincrement { get; set; }

        private DbEntityInfo()
        {

        }

        public DbEntityInfo(KeyColumn[] primaryKeyColumns, bool autoincrement, params KeyColumn[][] uniqueConstraints)
        {
            Autoincrement = autoincrement;
            PrimaryKeyColumns = primaryKeyColumns;
            UniqueConstraints = uniqueConstraints;
        }
    }
}
