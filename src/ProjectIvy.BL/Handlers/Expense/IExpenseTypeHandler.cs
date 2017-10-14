﻿using ProjectIvy.Model.Binding.ExpenseType;
using ProjectIvy.Model.View.ExpenseType;
using ProjectIvy.Model.View;
using System.Collections.Generic;

namespace ProjectIvy.BL.Handlers.Expense
{
    public interface IExpenseTypeHandler : IHandler
    {
        IEnumerable<ExpenseType> Get(ExpenseTypeGetBinding binding);

        IEnumerable<Node<ExpenseType>> GetTree();
    }
}
