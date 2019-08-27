using Microsoft.SqlServer.Server;
using ProjectIvy.Common.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ProjectIvy.Data.Databases.Main.UserTypes
{
    public class IntList
    {
        public const string TypeName = "dbo.IntList";

        private readonly IEnumerable<IntListItem> _items;

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
