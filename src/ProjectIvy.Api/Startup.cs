using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;
using ProjectIvy.Api.Attributes;
using ProjectIvy.Api.Constants;
using ProjectIvy.Api.Extensions;
using ProjectIvy.Api.Services;
using ProjectIvy.Business.Handlers.Account;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Business.Handlers.Beer;
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
using ProjectIvy.Business.Handlers.Location;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Business.Handlers.PaymentType;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Business.Handlers.Ride;
using ProjectIvy.Business.Handlers.Stay;
using ProjectIvy.Business.Handlers.Tracking;
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

    public IConfigurationRoot Configuration { get; }

    public Startup(IWebHostEnvironment env)
    {
        _authority = Environment.GetEnvironmentVariable("OAUTH_AUTHORITY");
        var builder = new ConfigurationBuilder()
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        IdentityModelEventSource.ShowPII = true;
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
        services.AddHandler<IBeerHandler, BeerHandler>();
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
        services.AddHandler<ILastFmHandler, LastFmHandler>();
        services.AddHandler<ILocationHandler, LocationHandler>();
        services.AddHandler<IMovieHandler, MovieHandler>();
        services.AddHandler<IPaymentTypeHandler, PaymentTypeHandler>();
        services.AddHandler<IPoiHandler, PoiHandler>();
        services.AddHandler<IRideHandler, RideHandler>();
        services.AddHandler<IRouteHandler, RouteHandler>();
        services.AddHandler<IStayHandler, StayHandler>();
        services.AddHandler<ITrackingHandler, TrackingHandler>();
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
                                AuthorizationUrl = new Uri($"{_authority}/connect/authorize"),
                                TokenUrl = new Uri($"{_authority}/connect/token"),
                            }
                        }
                    });
                });

        // Register the individual schemes first (Keycloak JWT + MCP)
        services.AddKeycloakWebApiAuthentication(Configuration, o =>
        {
            o.RequireHttpsMetadata = false;
            
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["AccessToken"];
                    if (string.IsNullOrEmpty(token))
                    {
                        token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
                    }
                    
                    // Check if this is the special token - if so, disable lifetime validation
                    if (!string.IsNullOrEmpty(token) && token.EndsWith("x2DI4Q"))
                    {
                        Log.Information("Special token ending with x2DI4Q detected in OnMessageReceived - will disable lifetime validation");
                        context.Options.TokenValidationParameters.ValidateLifetime = false;
                    }
                    
                    context.Token = context.Request.Cookies["AccessToken"];
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var token = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                    if (token != null && token.RawData.EndsWith("x2DI4Q"))
                    {
                        Log.Information("Special token ending with x2DI4Q successfully validated");
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception != null)
                    {
                        Log.Warning("Authentication failed: {ExceptionType} - {Message}", 
                            context.Exception.GetType().Name, 
                            context.Exception.Message);
                    }
                    Console.WriteLine(context.Exception);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthentication(options =>
        {
            // Use a policy scheme that can decide at runtime which underlying scheme to use
            options.DefaultScheme = "dynamic";
            options.DefaultAuthenticateScheme = "dynamic";
            options.DefaultChallengeScheme = "dynamic";
        })
        .AddPolicyScheme("dynamic", "Dynamic Auth (JWT or MCP)", policy =>
        {
            policy.ForwardDefaultSelector = context =>
            {
                // Heuristic 1: MCP endpoint path (adjust if MapMcp uses a different base path)
                var path = context.Request.Path.Value?.ToLowerInvariant();
                string chosen;
                if (path != null && path.StartsWith("/mcp"))
                {
                    chosen = ModelContextProtocol.AspNetCore.Authentication.McpAuthenticationDefaults.AuthenticationScheme;
                    context.Items["ChosenAuthScheme"] = chosen;
                    Log.Debug("Dynamic auth selector chose {Scheme} based on path {Path}", chosen, path);
                    return chosen;
                }

                // Heuristic 2: Check for a custom MCP header (if clients send one, e.g. X-MCP-Client)
                if (context.Request.Headers.ContainsKey("X-MCP-Client"))
                {
                    chosen = ModelContextProtocol.AspNetCore.Authentication.McpAuthenticationDefaults.AuthenticationScheme;
                    context.Items["ChosenAuthScheme"] = chosen;
                    Log.Debug("Dynamic auth selector chose {Scheme} based on header X-MCP-Client for path {Path}", chosen, path);
                    return chosen;
                }

                // Heuristic 3: Accept header indicates MCP media type
                var accept = context.Request.Headers["Accept"].ToString();
                if (!string.IsNullOrEmpty(accept) && accept.Contains("application/mcp", StringComparison.OrdinalIgnoreCase))
                {
                    chosen = ModelContextProtocol.AspNetCore.Authentication.McpAuthenticationDefaults.AuthenticationScheme;
                    context.Items["ChosenAuthScheme"] = chosen;
                    Log.Debug("Dynamic auth selector chose {Scheme} based on Accept header {Accept} for path {Path}", chosen, accept, path);
                    return chosen;
                }

                // Otherwise fall back to JWT (Keycloak)
                chosen = JwtBearerDefaults.AuthenticationScheme;
                context.Items["ChosenAuthScheme"] = chosen;
                Log.Debug("Dynamic auth selector defaulted to {Scheme} for path {Path}", chosen, path);
                return chosen;
            };
        })
        .AddMcp();

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

                // Policy that explicitly requires the MCP scheme (for endpoints that should not accept JWT)
                options.AddPolicy("McpAccess", builder =>
                {
                    builder.AddAuthenticationSchemes(ModelContextProtocol.AspNetCore.Authentication.McpAuthenticationDefaults.AuthenticationScheme)
                           .RequireAuthenticatedUser();
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
                .WithHttpTransport()
                .WithToolsFromAssembly();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseSerilogRequestLogging(configure =>
        {
            configure.EnrichDiagnosticContext = (context, httpContext) =>
            {
                string authorizationValue = httpContext.Request.Headers["Authorization"];
                string cookieTokenValue = httpContext.Request.Cookies["Token"];
                string token = authorizationValue ?? cookieTokenValue;

                if (token is not null)
                {
                    string maskedToken = $"*****{token[^6..]}";
                    context.Set("Token", maskedToken);
                }
                context.Set("Version", GetType().Assembly.GetName().Version.ToString());

                // Add all HTTP request headers as header_{headerName}
                foreach (var header in httpContext.Request.Headers)
                {
                    string headerName = header.Key.Replace("-", "");
                    string headerValue = header.Value.ToString();
                    
                    // Mask sensitive headers
                    if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(headerValue))
                    {
                        headerValue = headerValue.Length > 6 ? $"*****{headerValue[^6..]}" : "*****";
                    }
                    else if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(headerValue))
                    {
                        headerValue = "*****";
                    }
                    
                    context.Set($"header_{headerName}", headerValue);
                }
            };
        });

        app.UseRouting();
        app.UseCors(builder => builder.SetIsOriginAllowed(origin => true).AllowCredentials().AllowAnyHeader().AllowAnyMethod());
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
        app.UseExceptionHandling();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMcp();
        });
    }
}
