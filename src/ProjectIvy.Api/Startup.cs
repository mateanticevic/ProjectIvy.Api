using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Business.Handlers.Application;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Card;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Business.Handlers.Currency;
using ProjectIvy.Business.Handlers.Device;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Business.Handlers.PaymentType;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Business.Handlers.Project;
using ProjectIvy.Business.Handlers.Security;
using ProjectIvy.Business.Handlers.Task;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.Trip;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Business.Handlers.Web;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Model.Constants;
using LastFm = ProjectIvy.Data.Services.LastFm;
using AzureStorage = ProjectIvy.Data.Services.AzureStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ProjectIvy.Api.Extensions;
using System;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.File;
using ProjectIvy.Business.Handlers.Flight;
using Swashbuckle.AspNetCore.Swagger;
using ProjectIvy.Business.Handlers.Call;

namespace ProjectIvy.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        protected AppSettings Settings { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ProjectIvy Api", Version = "v1" });
            });

            services.AddLogging()
                    .Configure<AppSettings>(Configuration);

            services.AddSingletonFactory<LastFm.IUserHelper, LastFm.UserFactory>();
            services.AddSingletonFactory<AzureStorage.IAzureStorageHelper, AzureStorage.AzureStorageFactory>();

            services.AddHandler<ILastFmHandler, LastFmHandler>();

            services.AddHandler<IAirportHandler, AirportHandler>();
            services.AddHandler<IApplicationHandler, ApplicationHandler>();
            services.AddHandler<IBeerHandler, BeerHandler>();
            services.AddHandler<ICallHandler, CallHandler>();
            services.AddHandler<ICarHandler, CarHandler>();
            services.AddHandler<ICardHandler, CardHandler>();
            services.AddHandler<IConsumationHandler, ConsumationHandler>();
            services.AddHandler<ICountryHandler, CountryHandler>();
            services.AddHandler<ICurrencyHandler, CurrencyHandler>();
            services.AddHandler<IDeviceHandler, DeviceHandler>();
            services.AddHandler<IFileHandler, FileHandler>();
            services.AddHandler<IFlightHandler, FlightHandler>();
            services.AddHandler<IExpenseHandler, ExpenseHandler>();
            services.AddHandler<IExpenseTypeHandler, ExpenseTypeHandler>();
            services.AddHandler<IIncomeHandler, IncomeHandler>();
            services.AddHandler<IMovieHandler, MovieHandler>();
            services.AddHandler<IPaymentTypeHandler, PaymentTypeHandler>();
            services.AddHandler<IPoiHandler, PoiHandler>();
            services.AddHandler<IProjectHandler, ProjectHandler>();
            services.AddHandler<ISecurityHandler, SecurityHandler>();
            services.AddHandler<ITaskHandler, TaskHandler>();
            services.AddHandler<ITrackingHandler, TrackingHandler>();
            services.AddHandler<ITripHandler, TripHandler>();
            services.AddHandler<IUserHandler, UserHandler>();
            services.AddHandler<IVendorHandler, VendorHandler>();
            services.AddHandler<IWebHandler, WebHandler>();

            services.AddMvc()
                    .AddXmlDataContractSerializerFormatters();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            var logger = loggerFactory.CreateLogger(nameof(Startup));
            logger.LogInformation((int)LogEvent.ApiInitiated, "Started!");

            app.UseStaticFiles();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectIvy V1");
            });

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseExceptionHandlingMiddleware();
            app.UseAuthenticationMiddleware();
            app.UseMvc();
        }
    }
}
