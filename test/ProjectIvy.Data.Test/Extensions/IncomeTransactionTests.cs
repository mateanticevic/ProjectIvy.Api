using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Test.Extensions;

public class IncomeTransactionTests
{
    [Fact]
    public void IncomeTransactionTracksCompleteLinksWithBothForeignKeys()
    {
        using var db = new MainContext("Server=localhost;Database=ModelOnly;Integrated Security=true");
        var entry = db.IncomeTransactions.Add(new IncomeTransaction { IncomeId = 1, TransactionId = 2 });
        var entity = entry.Metadata;

        Assert.Equal(EntityState.Added, entry.State);
        Assert.Equal("Finance", entity.GetSchema());
        Assert.Equal("IncomeTransaction", entity.GetTableName());
        Assert.Equal(new[] { "IncomeId", "TransactionId" },
            entity.FindPrimaryKey()!.Properties.Select(x => x.Name));
        Assert.Contains(entity.GetForeignKeys(), key =>
            key.Properties.Single().Name == nameof(IncomeTransaction.IncomeId)
            && key.PrincipalEntityType.ClrType == typeof(Income));
        Assert.Contains(entity.GetForeignKeys(), key =>
            key.Properties.Single().Name == nameof(IncomeTransaction.TransactionId)
            && key.PrincipalEntityType.ClrType == typeof(Transaction));
    }

    [Fact]
    public void LinkedTransactionQueryTranslates()
    {
        using var db = new MainContext("Server=localhost;Database=ModelOnly;Integrated Security=true");
        var incomeIds = new[] { "2026-11-03-1" };

        var sql = db.IncomeTransactions
                    .Where(x => x.IncomeId != null && x.TransactionId != null)
                    .Where(x => x.Income.UserId == 1 && incomeIds.Contains(x.Income.ValueId))
                    .Where(x => x.Transaction.Account.UserId == 1)
                    .Select(x => new
                    {
                        IncomeId = x.Income.ValueId,
                        Transaction = new Model.View.Account.Transaction(x.Transaction)
                    })
                    .ToQueryString();

        Assert.Contains("[Finance].[IncomeTransaction]", sql);
        Assert.Contains("[Finance].[Income]", sql);
        Assert.Contains("[Finance].[Transaction]", sql);
        Assert.Contains("[Finance].[Account]", sql);
    }
}
