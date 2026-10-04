using System.IO.Compression;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using Testcontainers.MsSql;

namespace ProjectIvy.EndToEnd;

public static class SqlServerDatabase
{
    public const string Image = "mcr.microsoft.com/mssql/server:2025-CU9-ubuntu-24.04";

    public static MsSqlContainer BuildContainer() => new MsSqlBuilder(Image)
        .WithPassword($"Ivy!{Guid.NewGuid():N}")
        .WithEnvironment("MSSQL_PID", "Developer")
        .Build();

    public static string ConnectionString(MsSqlContainer container, string database)
    {
        var builder = new SqlConnectionStringBuilder(container.GetConnectionString())
        {
            InitialCatalog = database,
            ConnectTimeout = 30
        };
        return builder.ConnectionString;
    }

    // Every write target must come from our running container, never the ambient environment.
    public static void ValidateTarget(string connectionString, string containerConnectionString, string database)
    {
        var actual = new SqlConnectionStringBuilder(connectionString);
        var expected = new SqlConnectionStringBuilder(containerConnectionString);
        if (!database.StartsWith("ivy_e2e_", StringComparison.Ordinal)
            || actual.DataSource != expected.DataSource
            || actual.InitialCatalog != database
            || actual.UserID != expected.UserID
            || actual.Password != expected.Password
            || actual.IntegratedSecurity)
        {
            throw new InvalidOperationException("Database target does not match the disposable SQL Server fixture.");
        }
    }

    public static void Deploy(string snapshotPath, string connectionString, string containerConnectionString, string database)
    {
        ValidateTarget(connectionString, containerConnectionString, database);
        ValidateMetadataOnly(snapshotPath);
        using var package = DacPackage.Load(snapshotPath);
        var options = new DacDeployOptions
        {
            CreateNewDatabase = true,
            CommandTimeout = 180,
            IgnoreAuthorizer = true,
            ExcludeObjectTypes =
            [
                ObjectType.Users, ObjectType.Logins, ObjectType.Permissions,
                ObjectType.ApplicationRoles, ObjectType.DatabaseRoles, ObjectType.ServerRoles,
                ObjectType.RoleMembership, ObjectType.ServerRoleMembership,
                ObjectType.LinkedServers, ObjectType.LinkedServerLogins,
                ObjectType.Credentials, ObjectType.DatabaseScopedCredentials,
                ObjectType.ExternalDataSources, ObjectType.ExternalTables,
                ObjectType.RemoteServiceBindings, ObjectType.Routes
            ]
        };
        new DacServices(connectionString).Deploy(package, database, upgradeExisting: false, options);
    }

    public static void ValidateMetadataOnly(string snapshotPath)
    {
        using var archive = ZipFile.OpenRead(snapshotPath);
        if (archive.Entries.Any(entry => entry.FullName.StartsWith("Data/", StringComparison.OrdinalIgnoreCase)
            || entry.FullName.EndsWith(".bcp", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("The schema snapshot contains table data. Refresh it with table-data extraction disabled.");
    }

    public static async Task DropAsync(string connectionString, string containerConnectionString, string database)
    {
        ValidateTarget(connectionString, containerConnectionString, database);
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        SqlConnection.ClearAllPools();
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        // Identifier is generated locally; escape it anyway rather than trusting interpolation.
        var quotedName = $"[{database.Replace("]", "]]")}]";
        command.CommandText = $"IF DB_ID(@database) IS NOT NULL BEGIN ALTER DATABASE {quotedName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {quotedName}; END";
        command.Parameters.AddWithValue("@database", database);
        await command.ExecuteNonQueryAsync();
    }
}
