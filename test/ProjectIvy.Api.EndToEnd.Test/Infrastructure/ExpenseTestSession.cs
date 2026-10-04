using System.Globalization;
using Microsoft.Data.SqlClient;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.EndToEnd;
using ProjectIvy.Model.Database.Main.Common;
using ProjectIvy.Model.Database.Main.Finance;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Api.EndToEnd.Test.Infrastructure;

public sealed class ExpenseTestSession : IAsyncDisposable
{
    private readonly SqlServerFixture _fixture;
    private readonly string _database = $"ivy_e2e_{Guid.NewGuid():N}";
    private TestEnvironment? _environment;
    private bool _deploymentAttempted;

    public ExpenseTestSession(SqlServerFixture fixture)
    {
        _fixture = fixture;
        ConnectionString = SqlServerDatabase.ConnectionString(fixture.Container, _database);
    }

    public string ConnectionString { get; }
    public ExpenseApiFactory? Factory { get; private set; }
    public User User { get; private set; } = null!;
    public User OtherUser { get; private set; } = null!;
    public int CurrencyId { get; private set; }
    public int ExpenseTypeId { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            SqlServerDatabase.ValidateTarget(ConnectionString, _fixture.Container.GetConnectionString(), _database);
            _deploymentAttempted = true;
            await Task.Run(() => SqlServerDatabase.Deploy(_fixture.SnapshotPath, ConnectionString,
                _fixture.Container.GetConnectionString(), _database));
            await SeedAsync();
            _environment = new TestEnvironment(ConnectionString);
            Factory = new ExpenseApiFactory(ConnectionString, _fixture.Container.GetConnectionString(), _database);
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public MainContext OpenDatabase() => new(ConnectionString);

    public async Task SeedExpenseAsync(int userId, string valueId, DateTime date, decimal amount)
    {
        using var db = OpenDatabase();
        db.Expenses.Add(new Expense
        {
            UserId = userId, ValueId = valueId, Amount = amount,
            CurrencyId = CurrencyId, ExpenseTypeId = ExpenseTypeId,
            Date = date, DatePaid = date, Comment = "Seeded expense"
        });
        await db.SaveChangesAsync();
    }

    private async Task SeedAsync()
    {
        using var db = OpenDatabase();
        var currency = new Currency { Code = "EUR", ValueId = "eur", Name = "Euro", Symbol = "€" };
        var type = new ExpenseType { ValueId = "e2e-food", Name = "Test food" };
        db.Currencies.Add(currency);
        db.ExpenseTypes.Add(type);
        await db.SaveChangesAsync();
        CurrencyId = currency.Id;
        ExpenseTypeId = type.Id;
        var languageId = await SeedLanguageAsync();
        User = NewUser("primary", languageId);
        OtherUser = NewUser("other", languageId);
        db.Users.AddRange(User, OtherUser);
        await db.SaveChangesAsync();
    }

    private User NewUser(string prefix, int languageId)
    {
        var token = Guid.NewGuid().ToString("N")[..16];
        return new User
        {
            Email = $"{prefix}-{token}@example.test", Username = $"e2e-{token}",
            FirstName = "Test", LastName = "User",
            DefaultCurrencyId = CurrencyId, DefaultLanguageId = languageId,
            Created = new DateTime(2026, 1, 1), Modified = new DateTime(2026, 1, 1)
        };
    }

    // Language is not mapped by MainContext. Read its FK metadata from the local snapshot,
    // rather than assuming a production lookup row exists or disabling its constraint.
    private async Task<int> SeedLanguageAsync()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var lookup = connection.CreateCommand();
        lookup.CommandText = """
            SELECT OBJECT_SCHEMA_NAME(f.referenced_object_id), OBJECT_NAME(f.referenced_object_id),
                   COL_NAME(f.referenced_object_id, f.referenced_column_id)
            FROM sys.foreign_key_columns f
            WHERE f.parent_object_id = OBJECT_ID('[User].[User]')
              AND COL_NAME(f.parent_object_id, f.parent_column_id) = 'DefaultLanguageId'
            """;
        string schema, table, key;
        await using (var reader = await lookup.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
                return 1; // No language FK in this schema; the required scalar still needs a value.
            schema = reader.GetString(0);
            table = reader.GetString(1);
            key = reader.GetString(2);
        }

        static string Quote(string name) => $"[{name.Replace("]", "]]")}]";
        var tableName = $"{Quote(schema)}.{Quote(table)}";
        await using var columns = connection.CreateCommand();
        columns.CommandText = """
            SELECT c.name, TYPE_NAME(c.system_type_id), c.max_length, c.is_nullable,
                   c.is_identity, c.is_computed, c.default_object_id
            FROM sys.columns c WHERE c.object_id = OBJECT_ID(@table) ORDER BY c.column_id
            """;
        columns.Parameters.AddWithValue("@table", tableName);
        var values = new List<(string Name, object Value)>();
        await using (var reader = await columns.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var name = reader.GetString(0);
                var type = reader.GetString(1);
                if (reader.GetBoolean(4) || reader.GetBoolean(5) || reader.GetInt32(6) != 0)
                    continue;
                if (reader.GetBoolean(3) && name is not ("Name" or "Code" or "ValueId"))
                    continue;
                object value = type switch
                {
                    "nvarchar" or "nchar" or "varchar" or "char" => "en",
                    "int" or "smallint" or "tinyint" when name == key => 1,
                    "bit" => false,
                    "datetime" or "datetime2" or "date" => new DateTime(2026, 1, 1),
                    _ => throw new InvalidOperationException($"Add a synthetic language seed for required column {tableName}.{name} ({type}).")
                };
                if (value is string text && reader.GetInt16(2) > 0)
                {
                    var length = reader.GetInt16(2) / (type.StartsWith('n') ? 2 : 1);
                    value = text[..Math.Min(text.Length, length)];
                }
                values.Add((name, value));
            }
        }

        await using var insert = connection.CreateCommand();
        var parameters = values.Select((_, i) => $"@p{i}").ToArray();
        insert.CommandText = values.Count == 0
            ? $"INSERT INTO {tableName} OUTPUT INSERTED.{Quote(key)} DEFAULT VALUES"
            : $"INSERT INTO {tableName} ({string.Join(",", values.Select(x => Quote(x.Name)))}) OUTPUT INSERTED.{Quote(key)} VALUES ({string.Join(",", parameters)})";
        for (var i = 0; i < values.Count; i++)
            insert.Parameters.AddWithValue(parameters[i], values[i].Value);
        return Convert.ToInt32(await insert.ExecuteScalarAsync(), CultureInfo.InvariantCulture);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (Factory is not null)
            {
                try { Factory.VerifyNoExternalCalls(); }
                finally { await Factory.DisposeAsync(); }
            }
        }
        finally
        {
            Factory = null;
            try
            {
                if (_deploymentAttempted)
                    await SqlServerDatabase.DropAsync(ConnectionString, _fixture.Container.GetConnectionString(), _database);
            }
            finally
            {
                _deploymentAttempted = false;
                _environment?.Dispose();
                _environment = null;
            }
        }
    }
}
