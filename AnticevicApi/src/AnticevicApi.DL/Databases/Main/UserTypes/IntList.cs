using AnticevicApi.Common.Extensions;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace AnticevicApi.DL.Databases.Main.UserTypes
{
    public class IntList
    {
        private readonly IEnumerable<IntListItem> _items;

        public const string TypeName = "dbo.IntList";
        
        public IntList(IEnumerable<int> items)
        {
            _items = items.IsNullOrEmpty() ? new List<IntListItem>() : items.Select(x => new IntListItem() { Value = x });
        }

        public IntList(IEnumerable<IntListItem> items)
        {
            _items = items;
        }

        public SqlParameter ToSqlParameter(string parameterName)
        {
            var items = new List<SqlDataRecord>();
            foreach (var item in _items.EmptyIfNull())
            {
                var record = new SqlDataRecord(new SqlMetaData(nameof(IntListItem.Value), SqlDbType.Int));
                record.SetInt32(0, item.Value);
                items.Add(record);
            }
            var sqlParameter = new SqlParameter(parameterName, SqlDbType.Structured);
            sqlParameter.Direction = ParameterDirection.Input;
            sqlParameter.TypeName = TypeName;
            sqlParameter.Value = items.IsNullOrEmpty() ? null : items;

            return sqlParameter;
        }
    }
}
