using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ProjectIvy.Data.Extensions;

public static class ObjectExtensions
{
    public static SqlParameter ToSqlParameter<T>(this T o, string parameterName)
    {
        var sqlParameter = new SqlParameter(parameterName, o)
        {
            Direction = ParameterDirection.Input,
            SqlValue = o == null ? DBNull.Value : (object)o
        };

        return sqlParameter;
    }
}
