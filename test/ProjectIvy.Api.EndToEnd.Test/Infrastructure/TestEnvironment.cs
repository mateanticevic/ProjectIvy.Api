namespace ProjectIvy.Api.EndToEnd.Test.Infrastructure;

public sealed class TestEnvironment : IDisposable
{
    private readonly Dictionary<string, string?> _previous = new();

    public TestEnvironment(string connectionString)
    {
        Set("CONNECTION_STRING_MAIN", connectionString);
        Set("CONNECTION_STRING_AZURE_STORAGE",
            $"AccountName=e2e;AccountKey={Convert.ToBase64String(new byte[64])};FileEndpoint=http://127.0.0.1:1/e2e");
        Set("LAST_FM_KEY", "unused-e2e-key");
        Set("Mcp__Resource", "https://localhost/mcp");
        Set("OAUTH_AUTHORITY", "https://127.0.0.1:1/realms/e2e");
        Set("Keycloak__auth-server-url", "https://127.0.0.1:1/");
        Set("Keycloak__realm", "e2e");
        Set("Keycloak__resource", "api");
        Set("Authentication__Schemes__Bearer__Authority", "https://127.0.0.1:1/realms/e2e");
        Set("USE_HTTP2", null);
        // Export credentials have no purpose in an API test process.
        Set("E2E_SCHEMA_SOURCE_CONNECTION_STRING", null);
    }

    private void Set(string name, string? value)
    {
        _previous.Add(name, Environment.GetEnvironmentVariable(name));
        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose()
    {
        foreach (var (name, value) in _previous)
            Environment.SetEnvironmentVariable(name, value);
    }
}
