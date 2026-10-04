using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ModelContextProtocol.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Linq;
using ModelContextProtocol.AspNetCore.Authentication;
using Microsoft.OpenApi;
using ProjectIvy.Api.Attributes;
using ProjectIvy.Api.Constants;
using ProjectIvy.Api.Extensions;
using ProjectIvy.Api.Services;
using ProjectIvy.Business.Handlers.Account;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Business.Handlers.Bank;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Business.Handlers.Brand;
using ProjectIvy.Business.Handlers.Calendar;
using ProjectIvy.Business.Handlers.Call;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Card;
using ProjectIvy.Business.Handlers.City;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Business.Handlers.Currency;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.File;
using ProjectIvy.Business.Handlers.Flight;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Business.Handlers.Inventory;
using ProjectIvy.Business.Handlers.JournalEntry;
using ProjectIvy.Business.Handlers.Loan;
using ProjectIvy.Business.Handlers.Location;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Business.Handlers.PaymentType;
using ProjectIvy.Business.Handlers.Person;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Business.Handlers.Ride;
using ProjectIvy.Business.Handlers.Stay;
using ProjectIvy.Business.Handlers.Tag;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.ToDo;
using ProjectIvy.Business.Handlers.Trip;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Business.Handlers.Webhooks;
using ProjectIvy.Business.Handlers.WorkDay;
using ProjectIvy.Business.Services.Calendar;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Model.Converters;
using Prometheus;
using Serilog;
using AzureStorage = ProjectIvy.Data.Services.AzureStorage;
using LastFm = ProjectIvy.Data.Services.LastFm;

namespace ProjectIvy.Api;

public class Startup
{
    private readonly string _authority;

    public Startup(IWebHostEnvironment env)
    {
        _authority = Environment.GetEnvironmentVariable("OAUTH_AUTHORITY");
        if (!Uri.TryCreate(_authority, UriKind.Absolute, out var authorityUri) ||
            authorityUri.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(authorityUri.Query) ||
            !string.IsNullOrEmpty(authorityUri.Fragment) || !string.IsNullOrEmpty(authorityUri.UserInfo))
            throw new InvalidOperationException("OAUTH_AUTHORITY must be the HTTPS Keycloak realm issuer.");
        var builder = new ConfigurationBuilder()
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseExceptionHandling();

        app.UseSerilogRequestLoggingWithEnrichment(GetType().Assembly);

        app.UseRouting();
        app.UseCors(builder => builder
            .WithOrigins(Configuration.GetSection("Mcp:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader().AllowAnyMethod()
            .WithExposedHeaders("WWW-Authenticate", "Mcp-Session-Id", "MCP-Protocol-Version"));
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpMetrics();
        app.UseMetricServer();
        app.UseStaticFiles();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectIvy");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMcp("/mcp").RequireAuthorization("McpAccess");
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024;
            options.UseCaseSensitivePaths = true;
        });
        services.AddLogging();

        services.AddHttpClient();
        services.AddScoped<IIcsCalendarService, IcsCalendarService>();

        services.AddHttpContextAccessor();
        services.AddSingleton<AzureStorage.IAzureStorageHelper>(new AzureStorage.AzureStorageHelper(Environment.GetEnvironmentVariable("CONNECTION_STRING_AZURE_STORAGE")));
        services.AddSingleton<LastFm.IUserHelper>(new LastFm.UserHelper(Environment.GetEnvironmentVariable("LAST_FM_KEY")));

        services.AddHandler<IAccountHandler, AccountHandler>();
        services.AddHandler<IAirlineHandler, AirlineHandler>();
        services.AddHandler<IAirportHandler, AirportHandler>();
        services.AddHandler<IBankHandler, BankHandler>();
        services.AddHandler<IBeerHandler, BeerHandler>();
        services.AddHandler<IBrandHandler, BrandHandler>();
        services.AddHandler<ICalendarHandler, CalendarHandler>();
        services.AddHandler<ICallHandler, CallHandler>();
        services.AddHandler<ICarHandler, CarHandler>();
        services.AddHandler<ICardHandler, CardHandler>();
        services.AddHandler<ICityHandler, CityHandler>();
        services.AddHandler<IConsumationHandler, ConsumationHandler>();
        services.AddHandler<ICountryHandler, CountryHandler>();
        services.AddHandler<ICurrencyHandler, CurrencyHandler>();
        services.AddHandler<IDialogflowHandler, DialogflowHandler>();
        services.AddHandler<IExpenseHandler, ExpenseHandler>();
        services.AddHandler<IExpenseTypeHandler, ExpenseTypeHandler>();
        services.AddHandler<IFileHandler, FileHandler>();
        services.AddHandler<IFlightHandler, FlightHandler>();
        services.AddHandler<IGeohashHandler, GeohashHandler>();
        services.AddHandler<IIncomeHandler, IncomeHandler>();
        services.AddHandler<IInventoryHandler, InventoryHandler>();
        services.AddHandler<IJournalEntryHandler, JournalEntryHandler>();
        services.AddHandler<ILastFmHandler, LastFmHandler>();
        services.AddHandler<ILoanHandler, LoanHandler>();
        services.AddHandler<ILocationHandler, LocationHandler>();
        services.AddHandler<IMovieHandler, MovieHandler>();
        services.AddHandler<IPaymentTypeHandler, PaymentTypeHandler>();
        services.AddHandler<IPersonHandler, PersonHandler>();
        services.AddHandler<IPoiHandler, PoiHandler>();
        services.AddHandler<IRideHandler, RideHandler>();
        services.AddHandler<IRouteHandler, RouteHandler>();
        services.AddHandler<IStayHandler, StayHandler>();
        services.AddHandler<ITagHandler, TagHandler>();
        services.AddHandler<ITrackingHandler, TrackingHandler>();
        services.AddHandler<IToDoHandler, ToDoHandler>();
        services.AddHandler<ITripHandler, TripHandler>();
        services.AddHandler<IUserHandler, UserHandler>();
        services.AddHandler<IVendorHandler, VendorHandler>();
        services.AddHandler<IWorkDayHandler, WorkDayHandler>();
        services.AddSingleton<IAuthorizationHandler, ScopeRequirementHandler>();

        services.AddMemoryCache(setup =>
        {
            setup.TrackStatistics = true;
        });
        services.AddHostedService<MetricsBackgroundService>();

        // Enable endpoint routing (required for UseRouting/UseEndpoints middleware)
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
                });

        services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectIvy", Version = "v1" });
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{_authority}/protocol/openid-connect/auth"),
                                TokenUrl = new Uri($"{_authority}/protocol/openid-connect/token"),
                            }
                        }
                    });
                });

        var resource = Configuration["Mcp:Resource"];
        if (!Uri.TryCreate(resource, UriKind.Absolute, out var resourceUri) ||
            resourceUri.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(resourceUri.Fragment) ||
            !string.IsNullOrEmpty(resourceUri.Query) || !string.IsNullOrEmpty(resourceUri.UserInfo))
            throw new InvalidOperationException("Mcp:Resource must be the public HTTPS MCP URL (without query or fragment).");

        // Preserve the REST API's Keycloak issuer and audience configuration.
        services.AddKeycloakWebApiAuthentication(Configuration, o =>
        {
            o.MapInboundClaims = true;
            o.TokenValidationParameters.ValidateLifetime = true;
            o.TokenValidationParameters.ValidateIssuerSigningKey = true;
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (!context.Request.Headers.ContainsKey("Authorization"))
                        context.Token = context.Request.Cookies["AccessToken"];
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "api";
            options.DefaultAuthenticateScheme = "api";
            options.DefaultChallengeScheme = "challenge";
        })
        .AddPolicyScheme("api", "REST or MCP bearer validation", options =>
        {
            options.ForwardDefaultSelector = context => context.Request.Path.StartsWithSegments("/mcp")
                ? "McpBearer" : JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer("McpBearer", options =>
        {
            options.Authority = _authority;
            options.RequireHttpsMetadata = true;
            options.MapInboundClaims = true;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidAudience = resource;
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var emails = context.Principal.FindAll(ClaimTypes.Email).ToArray();
                    if (emails.Length != 1 || string.IsNullOrWhiteSpace(emails[0].Value))
                        context.Fail("An email claim is required.");
                    return Task.CompletedTask;
                }
            };
        })
        .AddPolicyScheme("challenge", "OAuth challenge", options =>
        {
            options.ForwardDefaultSelector = context => context.Request.Path.StartsWithSegments("/mcp")
                ? McpAuthenticationDefaults.AuthenticationScheme : JwtBearerDefaults.AuthenticationScheme;
        })
        .AddMcp(options =>
        {
            options.ResourceMetadata = new()
            {
                Resource = resource,
                AuthorizationServers = { _authority },
                ScopesSupported = [ApiScopes.ExpenseUser, ApiScopes.ExpenseCreate, ApiScopes.BeerUser]
            };
        });

        services.AddAuthorization(options =>
            {
                var fields = typeof(ApiScopes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);

                foreach (var field in fields)
                {
                    string value = field.GetValue(null).ToString();
                    options.AddPolicy(value, builder =>
                    {
                        builder.Requirements.Add(new ScopeRequirement(value));
                    });
                }

                // MCP uses the validated bearer identity and the MCP discovery challenge.
                options.AddPolicy("McpAccess", builder =>
                {
                    builder.RequireAuthenticatedUser();
                });

                // Policy that explicitly requires JWT (if you need to exclude MCP for some endpoints)
                options.AddPolicy("JwtAccess", builder =>
                {
                    builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                           .RequireAuthenticatedUser();
                });
            })
            .AddKeycloakAuthorization(Configuration)
            .AddAuthorizationBuilder();

        services.AddMvc(setup =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
            setup.Filters.Add(new AuthorizeFilter(policy));
        });

        services.AddMcpServer()
                .WithHttpTransport(options =>
                {
                    options.SessionMode = HttpServerSessionMode.Stateless;
                })
                .AddAuthorizationFilters()
                .WithToolsFromAssembly();
    }
}
