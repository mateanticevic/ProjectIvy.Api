using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.BL.Handlers.Application;
using AnticevicApi.BL.Handlers.Car;
using AnticevicApi.BL.Handlers.Country;
using AnticevicApi.BL.Handlers.Currency;
using AnticevicApi.BL.Handlers.Device;
using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.BL.Handlers.Project;
using AnticevicApi.BL.Handlers.Security;
using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.BL.Handlers.Trip;
using AnticevicApi.BL.Handlers.User;
using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.BL.Handlers.Web;
using AnticevicApi.BL.Services.LastFm;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Extensions;
using AnticevicApi.Model.Constants;
using LastFm = AnticevicApi.DL.Services.LastFm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace AnticevicApi
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

            services.AddLogging()
                    .Configure<AppSettings>(Configuration);

            services.AddSingletonFactory<LastFm.IUserHelper, LastFm.UserFactory>();

            services.AddHandler<ILastFmHandler, LastFmHandler>();

            services.AddHandler<IAirportHandler, AirportHandler>();
            services.AddHandler<IApplicationHandler, ApplicationHandler>();
            services.AddHandler<ICarHandler, CarHandler>();
            services.AddHandler<ICountryHandler, CountryHandler>();
            services.AddHandler<ICurrencyHandler, CurrencyHandler>();
            services.AddHandler<IDeviceHandler, DeviceHandler>();
            services.AddHandler<IExpenseHandler, ExpenseHandler>();
            services.AddHandler<IExpenseTypeHandler, ExpenseTypeHandler>();
            services.AddHandler<IIncomeHandler, IncomeHandler>();
            services.AddHandler<IMovieHandler, MovieHandler>();
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

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseExceptionHandlingMiddleware();
            app.UseAuthenticationMiddleware();
            app.UseMvc();
        }
    }
}
