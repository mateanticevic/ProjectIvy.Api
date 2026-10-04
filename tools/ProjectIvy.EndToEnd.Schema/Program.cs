using System.Diagnostics;
using Microsoft.Data.SqlClient;
using ProjectIvy.EndToEnd;

var source = Environment.GetEnvironmentVariable("E2E_SCHEMA_SOURCE_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(source) || args.Length != 1)
{
    Console.Error.WriteLine("Set E2E_SCHEMA_SOURCE_CONNECTION_STRING and run scripts/refresh-e2e-schema.sh. CONNECTION_STRING_MAIN is never used.");
    return 1;
}

// Parse without echoing credentials on malformed input.
try
{
    if (string.IsNullOrWhiteSpace(new SqlConnectionStringBuilder(source).InitialCatalog))
        throw new ArgumentException();
}
catch
{
    Console.Error.WriteLine("The schema source must be a valid connection string with an explicit database.");
    return 1;
}

var target = Path.GetFullPath(args[0]);
Directory.CreateDirectory(Path.GetDirectoryName(target)!);
// Same directory makes the final replacement atomic; a failed refresh leaves Main.dacpac intact.
var candidate = target + $".{Guid.NewGuid():N}.tmp.dacpac";
var stage = "starting disposable SQL Server 2025";
using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(10));
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };

try
{
    await using var container = SqlServerDatabase.BuildContainer();
    Console.WriteLine("Starting disposable SQL Server 2025 for schema validation...");
    await container.StartAsync(cancellation.Token);

    stage = "extracting schema with SqlPackage";
    Console.WriteLine("Extracting schema only (no table data)...");
    var start = new ProcessStartInfo("dotnet")
    {
        RedirectStandardOutput = true, RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var argument in new[]
    {
        "tool", "run", "sqlpackage", "--", "/Action:Extract",
        $"/SourceConnectionString:{source}", $"/TargetFile:{candidate}",
        "/p:ExtractAllTableData=false", "/p:ExtractApplicationScopedObjectsOnly=true",
        "/p:ExtractReferencedServerScopedElements=false", "/p:IgnorePermissions=true",
        "/p:IgnoreUserLoginMappings=true", "/p:VerifyExtraction=true"
    }) start.ArgumentList.Add(argument);

    using (var process = Process.Start(start) ?? throw new InvalidOperationException())
    {
        // Capture diagnostics and redact source details before displaying failures.
        var output = process.StandardOutput.ReadToEndAsync(cancellation.Token);
        var error = process.StandardError.ReadToEndAsync(cancellation.Token);
        try { await process.WaitForExitAsync(cancellation.Token); }
        catch
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
            throw;
        }
        await Task.WhenAll(output, error);
        if (process.ExitCode != 0)
        {
            Console.Error.WriteLine($"SqlPackage exited with code {process.ExitCode}:");
            Console.Error.WriteLine(SchemaDiagnostics.Redact(await output + Environment.NewLine + await error, source));
            throw new InvalidOperationException();
        }
    }

    stage = "deploying the extracted schema into SQL Server 2025";
    Console.WriteLine("Validating deployment into disposable SQL Server 2025...");
    var database = $"ivy_e2e_schema_{Guid.NewGuid():N}";
    var connectionString = SqlServerDatabase.ConnectionString(container, database);
    SqlServerDatabase.Deploy(candidate, connectionString, container.GetConnectionString(), database);

    stage = "replacing the schema snapshot";
    File.Move(candidate, target, overwrite: true);
    Console.WriteLine("Schema snapshot refreshed and validated. Review and commit Schema/Main.dacpac.");
    return 0;
}
catch (Exception)
{
    Console.Error.WriteLine($"Schema refresh failed while {stage}. The previous snapshot was preserved. Check Docker, source metadata permissions, SqlPackage installation, and schema compatibility.");
    return 1;
}
finally
{
    if (File.Exists(candidate)) File.Delete(candidate);
}
