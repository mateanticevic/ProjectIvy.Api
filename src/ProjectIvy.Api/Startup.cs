using ProjectIvy.BL.Handlers.Airport;
using ProjectIvy.BL.Handlers.Application;
using ProjectIvy.BL.Handlers.Car;
using ProjectIvy.BL.Handlers.Card;
using ProjectIvy.BL.Handlers.Country;
using ProjectIvy.BL.Handlers.Currency;
using ProjectIvy.BL.Handlers.Device;
using ProjectIvy.BL.Handlers.Expense;
using ProjectIvy.BL.Handlers.Income;
using ProjectIvy.BL.Handlers.Movie;
using ProjectIvy.BL.Handlers.PaymentType;
using ProjectIvy.BL.Handlers.Poi;
using ProjectIvy.BL.Handlers.Project;
using ProjectIvy.BL.Handlers.Security;
using ProjectIvy.BL.Handlers.Task;
using ProjectIvy.BL.Handlers.Tracking;
using ProjectIvy.BL.Handlers.Trip;
using ProjectIvy.BL.Handlers.User;
using ProjectIvy.BL.Handlers.Vendor;
using ProjectIvy.BL.Handlers.Web;
using ProjectIvy.BL.Services.LastFm;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Model.Constants;
using LastFm = ProjectIvy.DL.Services.LastFm;
using AzureStorage = ProjectIvy.DL.Services.AzureStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ProjectIvy.Api.Extensions;
using System;
using ProjectIvy.BL.Handlers.Beer;
using ProjectIvy.BL.Handlers.Consumation;
using ProjectIvy.BL.Handlers.File;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddHandler<ICarHandler, CarHandler>();
            services.AddHandler<ICardHandler, CardHandler>();
            services.AddHandler<IConsumationHandler, ConsumationHandler>();
            services.AddHandler<ICountryHandler, CountryHandler>();
            services.AddHandler<ICurrencyHandler, CurrencyHandler>();
            services.AddHandler<IDeviceHandler, DeviceHandler>();
            services.AddHandler<IFileHandler, FileHandler>();
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
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var logger = loggerFactory.CreateLogger(nameof(Startup));
            logger.LogInformation((int)LogEvent.ApiInitiated, "Started!");

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
