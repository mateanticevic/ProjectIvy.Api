using Moq;
using System.Net;
using System.Net.Http.Json;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.EndToEnd;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class SetupSafetyTests
{
    [Fact]
    public void SchemaErrorsPreserveTheCauseButRedactConnectionDetails()
    {
        const string source = "Server=tcp:private.example.test,1433;Database=PrivateDatabase;User Id=private-user;Password=Fake!Password123";
        var diagnostic = $"{source}\nLogin failed for user 'private-user'. Certificate chain was issued by an authority that is not trusted. Server private.example.test; database PrivateDatabase; password Fake!Password123.";
        var redacted = SchemaDiagnostics.Redact(diagnostic, source);
        Assert.DoesNotContain("Fake!Password123", redacted);
        Assert.DoesNotContain("private-user", redacted);
        Assert.DoesNotContain("private.example.test", redacted);
        Assert.DoesNotContain("PrivateDatabase", redacted);
        Assert.Contains("Login failed", redacted);
        Assert.Contains("Certificate chain was issued by an authority that is not trusted", redacted);
    }

    private const string ContainerConnection = "Server=localhost,49152;Database=master;User Id=sa;Password=Test!123456";

    [Theory]
    [InlineData("Server=production.invalid;Database=ivy_e2e_test;User Id=sa;Password=Test!123456", "ivy_e2e_test")]
    [InlineData("Server=localhost,49152;Database=production;User Id=sa;Password=Test!123456", "production")]
    [InlineData("Server=localhost,49152;Database=ivy_e2e_other;User Id=sa;Password=Test!123456", "ivy_e2e_test")]
    public void RejectsTargetsOutsideTheFixture(string target, string database)
        => Assert.Throws<InvalidOperationException>(() => SqlServerDatabase.ValidateTarget(target, ContainerConnection, database));

    [Fact]
    public void RejectsSnapshotsContainingTableData()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ivy-schema-{Guid.NewGuid():N}.dacpac");
        try
        {
            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
                archive.CreateEntry("Data/Finance.Expense/TableData.bcp");
            Assert.Throws<InvalidOperationException>(() => SqlServerDatabase.ValidateMetadataOnly(path));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void RestoresAmbientEnvironmentAfterSetupFailure()
    {
        var previous = Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN");
        var exportSource = Environment.GetEnvironmentVariable("E2E_SCHEMA_SOURCE_CONNECTION_STRING");
        Assert.Throws<InvalidOperationException>((Action)(() =>
        {
            using var environment = new TestEnvironment(ContainerConnection);
            Assert.Equal(ContainerConnection, Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
            Assert.Null(Environment.GetEnvironmentVariable("E2E_SCHEMA_SOURCE_CONNECTION_STRING"));
            throw new InvalidOperationException("Simulated host startup failure");
        }));
        Assert.Equal(previous, Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
        Assert.Equal(exportSource, Environment.GetEnvironmentVariable("E2E_SCHEMA_SOURCE_CONNECTION_STRING"));
    }

    [Fact]
    public async Task RejectsUnexpectedExternalCalls()
    {
        using var factory = new ExpenseApiFactory("", ContainerConnection, "ivy_e2e_test");
        await Assert.ThrowsAsync<MockException>(() => factory.Storage.Object.GetFile("unexpected"));
        await Assert.ThrowsAsync<MockException>(() => factory.LastFm.Object.GetTotalCount("unexpected"));
        await Assert.ThrowsAsync<MockException>(() => factory.Calendar.Object.GetEventsAsync("https://example.test", DateTime.MinValue, DateTime.MaxValue));
        using var client = new HttpClient(factory.OutboundHttp);
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync("https://example.test"));
        Assert.Equal(1, factory.OutboundHttp.Attempts);
        Assert.Throws<MockException>(factory.VerifyNoExternalCalls);
    }

    [Fact]
    public async Task HostUsesTestAuthenticationAndRejectsOutboundHttpWithoutADatabase()
    {
        // Authorization short-circuits these requests before handlers are created.
        // Port 1 is deliberately unreachable; no production setting is involved.
        const string container = "Server=127.0.0.1,1;Database=master;User Id=sa;Password=Unused!123456";
        const string target = "Server=127.0.0.1,1;Database=ivy_e2e_auth;User Id=sa;Password=Unused!123456";
        using var environment = new TestEnvironment(target);
        await using var factory = new ExpenseApiFactory(target, container, "ivy_e2e_auth");
        using var anonymous = factory.CreateUserClient(null, scope: null);
        using var noScope = factory.CreateUserClient("unseeded@example.test", scope: null);
        foreach (var client in new[] { anonymous, noScope })
        {
            var expected = client == anonymous ? HttpStatusCode.Unauthorized : HttpStatusCode.Forbidden;
            using var get = await client.GetAsync("/expense");
            using var post = await client.PostAsJsonAsync("/expense", new { });
            Assert.Equal(expected, get.StatusCode);
            Assert.Equal(expected, post.StatusCode);
        }
        factory.VerifyNoExternalCalls();

        using var external = factory.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        await Assert.ThrowsAsync<InvalidOperationException>(() => external.GetAsync("https://example.test"));
        Assert.Equal(1, factory.OutboundHttp.Attempts);
        Assert.Throws<InvalidOperationException>(factory.VerifyNoExternalCalls);
    }
}
