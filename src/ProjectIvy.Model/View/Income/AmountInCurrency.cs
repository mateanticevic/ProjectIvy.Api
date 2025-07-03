﻿using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Income;

public class AmountInCurrency
{
    public AmountInCurrency(decimal amount, DatabaseModel.Common.Currency currency)
    {
        Amount = amount;
        Currency = currency.ConvertTo(x => new Currency.Currency(x));
    }

    public decimal Amount { get; set; }

    public Currency.Currency Currency { get; set; }
}