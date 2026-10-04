using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Moq;
using ProjectIvy.Business.Services.Calendar;
using ProjectIvy.Data.Services.AzureStorage;
using ProjectIvy.EndToEnd;
using LastFm = ProjectIvy.Data.Services.LastFm;

namespace ProjectIvy.Api.EndToEnd.Test.Infrastructure;

public sealed class ExpenseApiFactory(string connectionString, string containerConnectionString, string database)
    : WebApplicationFactory<Program>
{
    public Mock<IAzureStorageHelper> Storage { get; } = new(MockBehavior.Strict);
    public Mock<LastFm.IUserHelper> LastFm { get; } = new(MockBehavior.Strict);
    public Mock<IIcsCalendarService> Calendar { get; } = new(MockBehavior.Strict);
    public RejectExternalHttpHandler OutboundHttp { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        SqlServerDatabase.ValidateTarget(connectionString, containerConnectionString, database);
        SqlServerDatabase.ValidateTarget(
            Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN") ?? "",
            containerConnectionString, database);

        builder.UseEnvironment("EndToEnd");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IAzureStorageHelper>();
            services.RemoveAll<LastFm.IUserHelper>();
            services.RemoveAll<IIcsCalendarService>();
            services.AddSingleton(Storage.Object);
            services.AddSingleton(LastFm.Object);
            services.AddSingleton(Calendar.Object);
            services.AddSingleton<IHttpMessageHandlerBuilderFilter>(new RejectExternalHttpFilter(OutboundHttp));

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.SchemeName, _ => { });
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthenticationHandler.SchemeName;
            });
        });
    }

    public HttpClient CreateUserClient(string? email, string? scope = "expense:user")
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        if (email is not null)
            client.DefaultRequestHeaders.Add("X-E2E-Email", email);
        if (scope is not null)
            client.DefaultRequestHeaders.Add("X-E2E-Scope", scope);
        return client;
    }

    public void VerifyNoExternalCalls()
    {
        Storage.VerifyNoOtherCalls();
        LastFm.VerifyNoOtherCalls();
        Calendar.VerifyNoOtherCalls();
        if (OutboundHttp.Attempts != 0)
            throw new InvalidOperationException("The API attempted an unexpected external HTTP request.");
    }
}

public sealed class RejectExternalHttpHandler : HttpMessageHandler
{
    private int _attempts;
    public int Attempts => _attempts;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _attempts);
        throw new InvalidOperationException("External HTTP requests are disabled in end-to-end tests.");
    }
}

internal sealed class RejectExternalHttpFilter(RejectExternalHttpHandler handler) : IHttpMessageHandlerBuilderFilter
{
    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) => builder =>
    {
        next(builder);
        builder.PrimaryHandler = handler;
    };
}
