﻿using ProjectIvy.DL.Databases.Main.UserTypes;
using ProjectIvy.DL.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;
using static Dapper.SqlMapper;

namespace ProjectIvy.DL.Databases.Main.Queries
{
    public class GetIncomeSumQuery : IDynamicParameters
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int UserId { get; set; }

        public int TargetCurrencyId { get; set; }

        public IEnumerable<int> IncomeIds { get; set; }

        public void AddParameters(IDbCommand command, Identity identity)
        {
            var sqlCommand = (SqlCommand)command;
            sqlCommand.CommandType = CommandType.Text;

            var intList = new IntList(IncomeIds);

            sqlCommand.Parameters.Add(intList.ToSqlParameter(nameof(IncomeIds)));
            sqlCommand.Parameters.Add(From.ToSqlParameter(nameof(From)));
            sqlCommand.Parameters.Add(To.ToSqlParameter(nameof(To)));
            sqlCommand.Parameters.Add(UserId.ToSqlParameter(nameof(UserId)));
            sqlCommand.Parameters.Add(TargetCurrencyId.ToSqlParameter(nameof(TargetCurrencyId)));
        }
    }
}
