using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Database.Main.Finance;
using System;

namespace AnticevicApi.BL.MapExtensions
{
    public static class ExpenseExtensions
    {
        public static Expense ToEntity(this ExpenseBinding binding, Expense entity = null)
        {
            if(entity == null)
            {
                entity = new Expense();
            }

            entity.Ammount = binding.Amount;
            entity.Comment = binding.Comment;
            entity.CurrencyId = CurrencyHelper.GetId(binding.CurrencyValueId);
            entity.Date = binding.Date;
            entity.ExpenseTypeId = ExpenseTypeHelper.GetId(binding.ExpenseTypeValueid).Value;
            entity.Modified = DateTime.Now;
            entity.VendorId = VendorHelper.GetId(binding.VendorValueId);

            return entity;
        }
    }
}
