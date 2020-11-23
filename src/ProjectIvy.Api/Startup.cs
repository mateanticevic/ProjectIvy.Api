using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ProjectIvy.Api.Extensions;
using ProjectIvy.Business.Handlers;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Business.Handlers.Application;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Business.Handlers.Call;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Card;
using ProjectIvy.Business.Handlers.City;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Business.Handlers.Currency;
using ProjectIvy.Business.Handlers.Device;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.File;
using ProjectIvy.Business.Handlers.Flight;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Business.Handlers.PaymentType;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Business.Handlers.Security;
using ProjectIvy.Business.Handlers.ToDo;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.Trip;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Business.Handlers.Web;
using ProjectIvy.Business.Handlers.Webhooks;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Model.Constants;
using System;
using System.Text.Json.Serialization;
using AzureStorage = ProjectIvy.Data.Services.AzureStorage;
using LastFm = ProjectIvy.Data.Services.LastFm;

namespace ProjectIvy.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();

            services.AddLogging();

            services.AddHttpContextAccessor();
            services.AddSingleton<ISecurityHandler, SecurityHandler>();
            services.AddSingleton<IHandlerContext<ISecurityHandler>, HandlerContext<ISecurityHandler>>();
            services.AddSingleton<AzureStorage.IAzureStorageHelper>(new AzureStorage.AzureStorageHelper(Environment.GetEnvironmentVariable("CONNECTION_STRING_AZURE_STORAGE")));
            services.AddSingleton<LastFm.IUserHelper>(new LastFm.UserHelper(Environment.GetEnvironmentVariable("LAST_FM_KEY")));

            services.AddHandler<ILastFmHandler, LastFmHandler>();
            services.AddHandler<IAirportHandler, AirportHandler>();
            services.AddHandler<IApplicationHandler, ApplicationHandler>();
            services.AddHandler<IBeerHandler, BeerHandler>();
            services.AddHandler<ICallHandler, CallHandler>();
            services.AddHandler<ICarHandler, CarHandler>();
            services.AddHandler<ICardHandler, CardHandler>();
            services.AddHandler<ICityHandler, CityHandler>();
            services.AddHandler<IConsumationHandler, ConsumationHandler>();
            services.AddHandler<ICountryHandler, CountryHandler>();
            services.AddHandler<ICurrencyHandler, CurrencyHandler>();
            services.AddHandler<IDeviceHandler, DeviceHandler>();
            services.AddHandler<IDialogflowHandler, DialogflowHandler>();
            services.AddHandler<IFileHandler, FileHandler>();
            services.AddHandler<IFlightHandler, FlightHandler>();
            services.AddHandler<IExpenseHandler, ExpenseHandler>();
            services.AddHandler<IExpenseTypeHandler, ExpenseTypeHandler>();
            services.AddHandler<IIncomeHandler, IncomeHandler>();
            services.AddHandler<IMovieHandler, MovieHandler>();
            services.AddHandler<IPaymentTypeHandler, PaymentTypeHandler>();
            services.AddHandler<IPoiHandler, PoiHandler>();
            services.AddHandler<IToDoHandler, ToDoHandler>();
            services.AddHandler<ITrackingHandler, TrackingHandler>();
            services.AddHandler<ITripHandler, TripHandler>();
            services.AddHandler<IUserHandler, UserHandler>();
            services.AddHandler<IVendorHandler, VendorHandler>();
            services.AddHandler<IWebHandler, WebHandler>();

            services.AddControllers(options => options.EnableEndpointRouting = false).AddNewtonsoftJson().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            var logger = loggerFactory.CreateLogger(nameof(Startup));
            logger.LogInformation((int)LogEvent.ApiInitiated, "Started!");

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectIvy V1");
            });

            app.UseCors(builder => builder.SetIsOriginAllowed(origin => true).AllowCredentials().AllowAnyHeader().AllowAnyMethod());

            app.UseExceptionHandlingMiddleware();
            app.UseAuthenticationMiddleware();
            app.UseMvc();
        }
    }
}
