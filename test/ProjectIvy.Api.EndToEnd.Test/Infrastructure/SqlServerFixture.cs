using ProjectIvy.EndToEnd;
using Testcontainers.MsSql;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test.Infrastructure;

[CollectionDefinition(Name)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
    public const string Name = "SQL Server end-to-end";
}

public sealed class SqlServerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = null!;
    public string SnapshotPath { get; } = Path.Combine(AppContext.BaseDirectory, "Schema", "Main.dacpac");

    public async Task InitializeAsync()
    {
        if (!File.Exists(SnapshotPath))
            throw new InvalidOperationException("Missing Schema/Main.dacpac. Run scripts/refresh-e2e-schema.sh with E2E_SCHEMA_SOURCE_CONNECTION_STRING set, then rebuild the test project. Tests never export production schema.");

        SqlServerDatabase.ValidateMetadataOnly(SnapshotPath);

        try
        {
            Container = SqlServerDatabase.BuildContainer();
            using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            await Container.StartAsync(timeout.Token);
        }
        catch
        {
            if (Container is not null) await Container.DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        if (Container is not null) await Container.DisposeAsync();
    }
}
