﻿using System.Data;
using Microsoft.Data.SqlClient;
using ProjectIvy.Data.Databases.Main.UserTypes;
using ProjectIvy.Data.Extensions;
using static Dapper.SqlMapper;

namespace ProjectIvy.Data.Databases.Main.Queries;

public class GetExpenseSumQuery : IDynamicParameters
{
    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public int? Month { get; set; }

    public int UserId { get; set; }

    public int TargetCurrencyId { get; set; }

    public string ExpenseTypeValueId { get; set; }

    public string VendorValueId { get; set; }

    public IEnumerable<int> ExpenseIds { get; set; }

    public void AddParameters(IDbCommand command, Identity identity)
    {
        var sqlCommand = (SqlCommand)command;
        sqlCommand.CommandType = CommandType.Text;

        var expenseIdList = new IntList(ExpenseIds);

        sqlCommand.Parameters.Add(expenseIdList.ToSqlParameter(nameof(ExpenseIds)));
        sqlCommand.Parameters.Add(From.ToSqlParameter(nameof(From)));
        sqlCommand.Parameters.Add(To.ToSqlParameter(nameof(To)));
        sqlCommand.Parameters.Add(Month.ToSqlParameter(nameof(Month)));
        sqlCommand.Parameters.Add(UserId.ToSqlParameter(nameof(UserId)));
        sqlCommand.Parameters.Add(TargetCurrencyId.ToSqlParameter(nameof(TargetCurrencyId)));
        sqlCommand.Parameters.Add(VendorValueId.ToSqlParameter(nameof(VendorValueId)));
        sqlCommand.Parameters.Add(ExpenseTypeValueId.ToSqlParameter(nameof(ExpenseTypeValueId)));
    }
}
