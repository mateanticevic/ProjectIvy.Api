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
using Microsoft.OpenApi.Models;
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
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.Trip;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Business.Handlers.Webhooks;
using ProjectIvy.Business.Handlers.WorkDay;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Model.Converters;
using Prometheus;
using Serilog;
using AzureStorage = ProjectIvy.Data.Services.AzureStorage;
using LastFm = ProjectIvy.Data.Services.LastFm;

namespace ProjectIvy.Api
{
    public class Startup
    {
        private readonly string _authority;

        public Startup(IWebHostEnvironment env)
        {
            _authority = Environment.GetEnvironmentVariable("OAUTH_AUTHORITY");
            var builder = new ConfigurationBuilder()
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                    .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1024;
                options.UseCaseSensitivePaths = true;
            });
            services.AddLogging();

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

            services.AddControllers(options => options.EnableEndpointRouting = false)
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

            services.AddKeycloakWebApiAuthentication(Configuration, o => {
                o.RequireHttpsMetadata = false;
                o.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    context.Token = context.Request.Cookies["AccessToken"];
                                    return Task.CompletedTask;
                                },
                                OnAuthenticationFailed = context =>
                                {
                                    Console.WriteLine(context.Exception);
                                    return Task.CompletedTask;
                                }
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

                };
            });
            app.UseCors(builder => builder.SetIsOriginAllowed(origin => true).AllowCredentials().AllowAnyHeader().AllowAnyMethod());

            app.UseDeveloperExceptionPage();
            app.UseExceptionHandling();

            app.UseMvc();
        }
    }
}
