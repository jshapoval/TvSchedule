using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;
using TvSchedule.Data.Entities;


namespace TvSchedule.Data.Data
{
    public class SqlDataContextBase<K, V> where V : IDbEntity
    {
        public class BulkOperation<V> : IDisposable where V : IDbEntity

        {
           public void Dispose() 
            {
                Commit();
                Console.Beep();
            }

            private DataTable _dataTable { get; set; }
            private SqlDataContextBase<K, V> _context { get; set; }

            public BulkOperation(SqlDataContextBase<K, V> ctx)
            {
                _context = ctx;
                _dataTable = new DataTable();
            }

            public void ColumnInputExpression()
            {
            }
            
            public void Merge(V value, Action<V> action)
            {
             AddOrUpdate(value, action,false);   
            }


            public void Update(V value, Action<V> action)
            {
                AddOrUpdate(value, action, true);
            }

            private void AddOrUpdate(V value, Action<V> action, bool onlyUpdate)
            {
                if (_context.EntityInfo.PrimaryKeyColumns.Length > 1 &&
                    _context.EntityInfo.Autoincrement)
                {
                    throw new ApplicationException("Autoincrement-key must be a single key");
                }

                var oldColumnValues = _context.GetValues(value);
                action(value);

                var newColumnValues = _context.GetValues(value);

                var emptyPK = false;

                if (_context.EntityInfo.PrimaryKeyColumns.Length == 1 && _context.EntityInfo.Autoincrement)
                {
                    if (!newColumnValues.ContainsKey(_context.EntityInfo.PrimaryKeyColumns[0].Name) || newColumnValues[_context.EntityInfo.PrimaryKeyColumns[0].Name].Equals(0))
                    {
                        emptyPK = true;
                    }
                }
                else
                {
                    foreach (var key in _context.EntityInfo.PrimaryKeyColumns)
                    {
                        if (newColumnValues[key.Name] == null)
                        {
                            throw new ApplicationException("Your key is not full");
                        }
                    }
                }
               
                var diffs = Compare(oldColumnValues, newColumnValues);

                for (int c = 0; c < _context.EntityInfo.PrimaryKeyColumns.Length; c++)
                {
                    if (!_dataTable.Columns.Contains(_context.EntityInfo.PrimaryKeyColumns[c].Name))
                    {
                        var keyColumns = new DataColumn[_context.EntityInfo.PrimaryKeyColumns.Length];

                        for (int i = 0; i < _context.EntityInfo.PrimaryKeyColumns.Length; i++)
                        {
                            var column = new DataColumn();
                            column.ColumnName = _context.EntityInfo.PrimaryKeyColumns[i].Name;

                            if (_context.EntityInfo.Autoincrement == true)
                            {
                                column.AutoIncrement = true;
                                column.AutoIncrementSeed = -1;
                                column.AutoIncrementStep = -1;
                            }

                            keyColumns[i] = column;

                            if (!_dataTable.Columns.Contains(_context.EntityInfo.PrimaryKeyColumns[i].Name))
                            {
                                _dataTable.Columns.Add(column);
                            }
                        }
                        _dataTable.PrimaryKey = keyColumns;
                    }
                }

                if (!_dataTable.Columns.Contains("Merge"))
                {
                    _dataTable.Columns.Add("Merge", typeof(int)).AllowDBNull = true;
                }

                foreach (var diff in diffs)
                {
                    if (!_dataTable.Columns.Contains(diff.Key))
                    {
                        _dataTable.Columns.Add($"{diff.Key}");
                        _dataTable.Columns.Add($"{diff.Key}_Changed");
                    }
                }

                var row = _dataTable.NewRow();

                if (emptyPK)
                {
                    foreach (var diff in diffs)
                    {
                        if (diff.Key == "StartDateUtc")
                        {
                            DateTime timeFormat = (DateTime)diff.Value;
                            row[diff.Key] = timeFormat.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        }
                        else
                        {
                            row[diff.Key] = diff.Value.ToString();
                        }
                        
                        if (_dataTable.Columns.Contains($"{diff.Key}_Changed"))
                        {
                            row[row.Table.Columns[$"{diff.Key}_Changed"]] = 1;
                        }
                        row[row.Table.Columns["Merge"]] = 0.ToString();
                    }
                    _dataTable.Rows.Add(row);
                }

                else
                {
                    var keys = new List<object>();

                    for (int k = 0; k < _context.EntityInfo.PrimaryKeyColumns.Length; k++)
                    {
                        keys.Add(diffs[
                            $"{_context.EntityInfo.PrimaryKeyColumns[k].Name}"]); 
                    }

                    var foundRow = _dataTable.Rows.Find(keys.ToArray());

                    if (foundRow is null && _context.EntityInfo.UniqueConstraints.Any())
                    {
                        foreach (var uc in _context.EntityInfo.UniqueConstraints)
                        {
                            var constraint = new object[_context.EntityInfo.UniqueConstraints.Length];

                            foundRow = _dataTable.Rows.Find(constraint);

                            if (foundRow != null)
                                break;
                        }
                    }

                    var index = _dataTable.Rows.IndexOf(foundRow);

                    if (foundRow != null)
                    {
                        foreach (var diff in diffs)
                        {
                            if (diff.Value.GetType() == typeof(DateTime))
                            {
                                DateTime timeFormat = (DateTime)diff.Value;
                                _dataTable.Rows[index][diff.Key] = timeFormat.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                            }
                            else
                            {
                                _dataTable.Rows[index][diff.Key] = diff.Value.ToString();
                            }
                         
                            if (_dataTable.Columns.Contains($"{diff.Key}_Changed"))
                            {
                                _dataTable.Rows[index][$"{diff.Key}_Changed"] = 1;
                            }
                            row[row.Table.Columns["Merge"]] = 1.ToString();
                        }
                    }

                    else
                    {
                        foreach (var diff in diffs)
                        {
                            if (diff.Value.GetType() == typeof(DateTime))
                            {
                                var timeFormat = (DateTime)diff.Value;
                                row[diff.Key] = timeFormat.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                            }
                            else
                            {
                                row[diff.Key] = diff.Value.ToString();
                            }

                            if (_dataTable.Columns.Contains($"{diff.Key}_Changed"))
                            {
                                row[$"{diff.Key}_Changed"] = 1;
                            }
                            row[row.Table.Columns["Merge"]] = 1.ToString();
                        }
                        _dataTable.Rows.Add(row);
                    }
                }
            }

            private void Commit()
            {
                string dataTableHash;
                var resultList = new List<string>();

                var columnNames = _dataTable.Columns.Cast<DataColumn>()
                    .Select(x => x.ColumnName)
                    .ToList();
                var columnTypes = _dataTable.Columns.Cast<DataColumn>()
                    .Select(x => x.DataType.ToString())
                    .ToList();
                var columnAllowNull = _dataTable.Columns.Cast<DataColumn>()
                    .Select(x => x.AllowDBNull.ToString())
                    .ToList();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    resultList.Add(String.Concat(columnNames[i], "-", columnTypes[i]));
                }

                resultList.Sort();
                dataTableHash = (String.Join("_", resultList)).GetHashCode().ToString();

                
                var columnNamesAndTypes = new List<string>();
                var columnNamesForUpdate = new List<string>();
                var columnNamesForInsert = new List<string>();
                var columnNamesForInsertSelect = new List<string>();

                 var columnsTypesForSql = columnTypes;

                for (int i = 0; i < columnsTypesForSql.Count; i++)
                {
                    if (columnsTypesForSql[i] == "System.Int32")
                    {
                        columnsTypesForSql[i] = "INT";
                    }

                    if (columnsTypesForSql[i] == "System.String")
                    {
                        columnsTypesForSql[i] = "NVARCHAR(512)";
                    }

                    if (columnsTypesForSql[i] == "System.Boolean")
                    {
                        columnsTypesForSql[i] = "BIT";
                    }

                    if (columnsTypesForSql[i] == "System.DateTime")
                    {
                        columnsTypesForSql[i] = "dateTime2(7)";
                    }
                }
                var columnAllowNullForSql = columnAllowNull;
                for (int i = 0; i < columnAllowNullForSql.Count; i++)
                {
                    if (columnAllowNullForSql[i] == "True")
                    {
                        columnAllowNullForSql[i] = "";
                    }
                    else
                    {
                        columnAllowNullForSql[i] = "NOT NULL";
                    }
                }

                for (int i = 0; i < columnNames.Count; i++)
                {
                    columnNamesAndTypes.Add(string.Concat("[",columnNames[i],"]", " ", columnsTypesForSql[i], " ", columnAllowNull[i]));
                }

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (!columnNames[i].Contains("_Changed") && !columnNames[i].Contains("Merge"))
                    {
                        bool existInPKC = false;
                        for (int j = 0; j < _context.EntityInfo.PrimaryKeyColumns.Length; j++)
                        {
                            if (_context.EntityInfo.PrimaryKeyColumns[j].Name.Contains(columnNames[i]))
                            {
                                existInPKC = true;
                            }
                        }

                        if (existInPKC == false)
                        {
                            columnNamesForUpdate.Add(string.Concat("t.", columnNames[i], " = IIF( t.", columnNames[i], "<>udt.", columnNames[i], ", udt.", columnNames[i], ", t.", columnNames[i], ")"));
                        }
                    }
                }

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (_context.EntityInfo.PrimaryKeyColumns.Length == 1 &&
                            _context.EntityInfo.Autoincrement)
                    {
                            if (!columnNames[i].Contains("_Changed") && !columnNames[i].Contains("Merge") &&
                                columnNames[i] != "Id")
                            {
                                columnNamesForInsertSelect.Add(string.Concat("d.", columnNames[i]));
                            }
                    }
                    else
                    {
                        if (!columnNames[i].Contains("_Changed") && !columnNames[i].Contains("Merge"))
                        {
                            columnNamesForInsertSelect.Add(string.Concat("d.", columnNames[i]));
                        }
                    }
                }

                var conditionsForJoin = new List<string>();
                for (int i = 0; i < _context.EntityInfo.PrimaryKeyColumns.Length; i++)
                {
                    conditionsForJoin.Add(string.Concat("d.", _context.EntityInfo.PrimaryKeyColumns[i].Name, " = target.", _context.EntityInfo.PrimaryKeyColumns[i].Name));
                }

                var conditionsForInsert = new List<string>();
                for (int i = 0; i < _context.EntityInfo.PrimaryKeyColumns.Length; i++)
                {
                    conditionsForInsert.Add(string.Concat("target.", _context.EntityInfo.PrimaryKeyColumns[i].Name, " IS NULL"));
                }

                var conditionsForUpdate = new List<string>();
                for (int i = 0; i < _context.EntityInfo.PrimaryKeyColumns.Length; i++)
                {
                    conditionsForUpdate.Add(string.Concat("t.", _context.EntityInfo.PrimaryKeyColumns[i].Name, " = udt.", _context.EntityInfo.PrimaryKeyColumns[i].Name));
                }
                

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (_context.EntityInfo.PrimaryKeyColumns.Length == 1 &&
                        _context.EntityInfo.Autoincrement)
                    {
                        if (!columnNames[i].Contains("_Changed") && !columnNames[i].Contains("Merge") && columnNames[i] != "Id")
                        {
                            columnNamesForInsert.Add(columnNames[i]);
                        }
                    }

                    else
                    {
                        if (!columnNames[i].Contains("_Changed") && !columnNames[i].Contains("Merge"))
                        {
                            columnNamesForInsert.Add(columnNames[i]);
                        }
                    }
                }

                //проверка на существование такого типа на сервере, если нет с таким хешем - то создать тип
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    var queryForCheckTableType = string.Format(
                        @"IF TYPE_ID (N'[{0}_{1}]') IS NULL
                        CREATE TYPE [{0}_{1}] AS TABLE(
                        {2} 
                        )", _context.TableName, dataTableHash, String.Join(",\n", columnNamesAndTypes 
                     ));

                    SqlCommand checkCommand = new SqlCommand(queryForCheckTableType, connection);
                     checkCommand.ExecuteNonQuery();

                    string query;
                    if (_context.EntityInfo.PrimaryKeyColumns.Length == 1 &&
                        _context.EntityInfo.Autoincrement)
                    {
                         query = string.Format(@"
                    UPDATE t 
                    SET {3}
                    FROM {0} t
                        INNER JOIN @data udt ON {4}

                    INSERT INTO {0} ({1})
                    SELECT {2} FROM @data d
                    WHERE d.[Merge] = 0 
                    ", _context.TableName, String.Join(",", columnNamesForInsert), String.Join(",", columnNamesForInsertSelect), String.Join(",\n", columnNamesForUpdate), String.Join(" AND ", conditionsForUpdate));

                    }
                    else
                    {
                         query = string.Format(@"
                    UPDATE t 
                    SET {3}
                    FROM {0} t
                        INNER JOIN @data udt ON {4}

                    INSERT INTO {0} ({1})
                    SELECT {2} FROM @data d
                    LEFT JOIN {0} target with(nolock) ON {5}
                    WHERE {6}
                    ", _context.TableName, String.Join(",", columnNamesForInsert), String.Join(",", columnNamesForInsertSelect), String.Join(",\n", columnNamesForUpdate), String.Join(" AND ", conditionsForUpdate), String.Join(" AND ", conditionsForJoin), String.Join(" AND ", conditionsForInsert));
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlParameter tvpParam = command.Parameters.AddWithValue("@data", _dataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = String.Concat("dbo.", _context.TableName, "_", dataTableHash);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Create(V value, Action<V> action)
        {
            action(value);

            var newColumnValues = GetValues(value);
            var columns = GetValues(value).Keys.ToList();
            var columnsForInsert = new List<string>();

            var valuesForInsert = new List<string>();

            foreach (var newColumnValue in newColumnValues)
            {
                if (newColumnValue.Value != null && newColumnValue.Value.ToString() != "0" && !newColumnValue.Value.Equals(default(DateTime)))
                {
                    if (newColumnValue.Value.GetType() == typeof(DateTime))
                    {
                        var timeFormat = (DateTime)newColumnValue.Value;
                        valuesForInsert.Add(string.Concat("'", timeFormat.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), "'"));
                    }

                    else if (newColumnValue.Value.GetType() == typeof(string))
                    {
                        valuesForInsert.Add(String.Concat("N'", newColumnValue.Value.ToString(), "'"));
                    }

                    else if (newColumnValue.Value.GetType() == typeof(bool))
                    {
                        if (newColumnValue.Value.Equals(false))
                        {
                            valuesForInsert.Add("0");
                        }
                        else
                        {
                            valuesForInsert.Add("1");
                        }
                        
                    }

                    else
                    {
                        valuesForInsert.Add(newColumnValue.Value.ToString());
                    }
                }
            }

            foreach (var column in columns)
            {
                object val;
                newColumnValues.TryGetValue($"{column}", out val);
                if (val!=null && val.ToString()!="0" && !val.Equals(default(DateTime)))
                {
                    if (EntityInfo.Autoincrement)
                    {
                        bool contain = false;
                        foreach (var pkc in EntityInfo.PrimaryKeyColumns)
                        {
                            if (pkc.Name.Contains(column))
                            {
                                contain = true;
                                break;
                            }
                        }

                        if (contain == false)
                        {
                            columnsForInsert.Add(column);
                        }
                    }
                    
                    else
                    {
                        columnsForInsert.Add(column);
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                var queryForInsert = string.Format(
                    @"INSERT INTO {0} ({1}) VALUES ({2})", TableName, String.Join(" ,",columnsForInsert), String.Join(" ,", valuesForInsert)
                );

                SqlCommand checkCommand = new SqlCommand(queryForInsert, connection);
                checkCommand.ExecuteNonQuery();
            }
        }


        public void Update(V value, Action<V> action)
        {
            UpdateProtected(value, action);
        }

        protected void UpdateProtected(V value, Action<V> action, bool releaseLock = false, string lockSessionId = null, string lockSessionIdFieldName = null, string lockExpireFieldName = null)
        {
            var oldColumnValues = GetValues(value);
            action(value);

            var newColumnValues = GetValues(value);
            var columnsAndValuesForInsert = new List<string>();

            var diffs = Compare(oldColumnValues, newColumnValues);

            foreach (var diff in diffs)
            {
               bool contain = false;
                foreach (var primaryKey in EntityInfo.PrimaryKeyColumns)
                {
                    if (primaryKey.Name.Contains(diff.Key))
                    {
                        contain = true;
                        break;
                    }
                }

                if (contain==false)
                {
                    if (diff.Value != null && diff.Value.ToString() != "0" &&
                                !diff.Value.Equals(default(DateTime)))
                    {
                    
                        if (diff.Value.GetType() == typeof(DateTime)) 
                        {
                             var timeFormat = (DateTime)diff.Value;
                             columnsAndValuesForInsert.Add(string.Concat(diff.Key," = '",
                             timeFormat.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), "'"));
                        }

                        else if (diff.Value.GetType() == typeof(string))
                        {
                            columnsAndValuesForInsert.Add(String.Concat(diff.Key, " = N'", diff.Value.ToString(), "'"));
                        }

                        else if (diff.Value.GetType() == typeof(bool))
                        {
                            if (diff.Value.Equals(false))
                            {
                                columnsAndValuesForInsert.Add(String.Concat(diff.Key, " = 0"));
                            }
                            else
                            {
                                columnsAndValuesForInsert.Add(String.Concat(diff.Key, " = 1"));
                            }
                        }

                        else
                        {
                            columnsAndValuesForInsert.Add(String.Concat(diff.Key, " = ", diff.Value));
                        }
                    }
                }
            }

            var conditions = new List<string>();
            for (int i = 0; i < EntityInfo.PrimaryKeyColumns.Length; i++)
            {
                object val = "";
                newColumnValues.TryGetValue($"{EntityInfo.PrimaryKeyColumns[i].Name}", out val);

                if (val.GetType()==typeof(string))
                {
                    conditions.Add(string.Concat(EntityInfo.PrimaryKeyColumns[i].Name, " = '", val , "'"));
                }

                else
                {
                    conditions.Add(string.Concat(EntityInfo.PrimaryKeyColumns[i].Name, " = ", val));
                }
            }

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                string queryForUpdate = null;
                SqlCommand command;

                if (releaseLock)//проверка блокировки и ее снятие 
                {
                    queryForUpdate = string.Format(
                        @"UPDATE {0} SET {1}, LockSessionId = null, LockExpirationUtc = null WHERE {2} AND LockSessionId IS NOT NULL OR LockSessionId = @LockSessionId AND LockExpirationUtc > GETUTCDATE()
                    set @ReturnValue = @@rowCount;", TableName, String.Join(" ,", columnsAndValuesForInsert),
                        String.Join(" AND ", conditions));

                     command = new SqlCommand(queryForUpdate, connection);
                    command.Parameters.Add("@LockSessionId", SqlDbType.NVarChar).Value = lockSessionId;

                    var returnValue = command.Parameters.Add("@ReturnValue", SqlDbType.Int);

                    returnValue.Direction = ParameterDirection.InputOutput;
                    returnValue.Value = -1;
                    var result = (int)command.Parameters["@ReturnValue"].Value;

                    if (result == 0)
                    {
                        throw new ApplicationException("0 rows have been updated!");
                    }
                }

                else
                {
                    queryForUpdate = string.Format(
                        @"UPDATE {0} SET {1} WHERE {2}", TableName, String.Join(" ,", columnsAndValuesForInsert),
                        String.Join(" AND ", conditions));
                     command = new SqlCommand(queryForUpdate, connection);
                }

                command.ExecuteNonQuery();
            }
        }

            public void Delete(V value)
            {
            var columnValues = GetValues(value);
            var conditions = new List<string>();
            for (int i = 0; i < EntityInfo.PrimaryKeyColumns.Length; i++)
            {
                object val;
                columnValues.TryGetValue($"{EntityInfo.PrimaryKeyColumns[i].Name}", out val);

                if (EntityInfo.PrimaryKeyColumns[i].Type.ToString() == "String")
                {
                    val = $"'{val}'";
                }

                conditions.Add(string.Concat(EntityInfo.PrimaryKeyColumns[i].Name, " = ", val));
            }

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                var queryForDelete = string.Format(
                    @"UPDATE {0} SET IsDeleted = 1 WHERE {1}", TableName, String.Join(" AND ", conditions)
                );
                

                SqlCommand checkCommand = new SqlCommand(queryForDelete, connection);
                checkCommand.ExecuteNonQuery();
            }
        }

        private static Dictionary<string, object> Compare(Dictionary<string, object> a, Dictionary<string, object> b) => b
            .Where(entry =>
            {
                if (entry.Value is int)
                {
                    return (int) a[entry.Key] != (int) entry.Value;
                }
                else if (entry.Value is DateTime)
                {
                    return (DateTime?)a[entry.Key] != (DateTime?)entry.Value;
                }
                else
                {
                    return a[entry.Key] != entry.Value;
                }                    
            })
            .ToDictionary(entry => entry.Key, entry => entry.Value);

        public virtual Dictionary<string, DataColumn> DbTypes
        {
            get { throw new NotImplementedException(); }
        }

        public const string CONNECTION_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TvSchedule;User Id=Julia;Password=a72a48a1000225;Integrated Security=False";

        protected virtual string TableName
        {
            get { throw new NotImplementedException(); }
        }

        protected virtual V CreateValue(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected DbEntityInfo EntityInfo { get; set; }

        protected virtual K GetKey(V value)
        {
            throw new NotImplementedException();
        }
      
        protected SqlDataContextBase(DbEntityInfo info)
        {
            EntityInfo = info;
        } 
        
        public BulkOperation<V> StartBulkProcessing()
        {
            return new BulkOperation<V>(this);
        }

        public List<V> GetList(int? count, params SqlFilter[] filters)
        {
            var query =
                $"select {(count.HasValue ? $"TOP {count.Value}" : null)} * from {TableName} where {DbHelper.GetWhereString(filters)}";

            var result = new List<V>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) 
                    {
                        while (reader.Read()) 
                        {
                            result.Add(CreateValue(reader));
                        }
                    };
                }
            }

            return result;
        }

        public V GetOne(K id)
        {
            var query = $"select top 1 * from {TableName} where Id = {id}";

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) 
                    {
                        while (reader.Read()) 
                        {
                            return CreateValue(reader);
                        }
                    }
                }
            }
            return default(V);
        }

        public void TryLock(V value, TimeSpan interval)
        {
            TryLockProtected(value, interval);
        }

        protected bool TryLockProtected(V value, TimeSpan interval, string lockSessionId = null, string lockSessionIdFieldName = null, string lockExpireFieldName = null)
        {
            var selectRowByPrimaryKey = "Id = " + GetKey(value);
            var query = string.Format(@"SELECT TOP 1 Id INTO #tmp FROM {0}
            WHERE {1} AND (LockSessionId IS NULL OR LockExpirationUtc IS NULL OR LockExpirationUtc < GETUTCDATE() OR LockSessionId = @lockSessionId)
	
            UPDATE tbl SET  LockSessionId =  @lockSessionId, LockExpirationUtc = @interval 
            from {0} tbl
            INNER JOIN #tmp t ON tbl.Id = t.Id 
            WHERE LockSessionId IS NULL OR LockExpirationUtc IS NULL OR LockExpirationUtc < GETUTCDATE() OR LockSessionId = @lockSessionId

            set @ReturnValue = @@rowCount;
            ", TableName, selectRowByPrimaryKey);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@lockSessionId", SqlDbType.NVarChar).Value = lockSessionId;
                command.Parameters.Add("@interval", SqlDbType.DateTime2).Value = DateTime.UtcNow.Add(interval);

                var returnValue = command.Parameters.Add("@ReturnValue", SqlDbType.Int);

                returnValue.Direction = ParameterDirection.InputOutput;
                returnValue.Value = -1;

                command.ExecuteNonQuery();

                var result = (int) command.Parameters["@ReturnValue"].Value;

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool TryRelease(V value, string lockSessionId)
        {
            var selectRowByPrimaryKey = "Id = " + GetKey(value);

            var query = string.Format(@"SELECT TOP 1 Id INTO #tmp FROM {0}
            WHERE {1} AND (LockSessionId IS NOT NULL OR LockSessionId = @LockSessionId)
	
            UPDATE {0} SET  LockSessionId =  @lockSessionId 
            INNER JOIN #tmp t ON {0}.Id = t.Id 
            WHERE LockSessionId IS NOT NULL OR LockSessionId = @LockSessionId

            RETURN @@rowCount", TableName, selectRowByPrimaryKey);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (command.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected virtual Dictionary<string, object> GetValues(V value)
        {
            throw new NotImplementedException();
        }
    }
}

