using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.BL.Handlers.Application;
using AnticevicApi.BL.Handlers.Car;
using AnticevicApi.BL.Handlers.Currency;
using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.BL.Handlers.Project;
using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.Config;
using AnticevicApi.Middleware;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AnticevicApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.Configure<AppSettings>(Configuration);

            services.AddScoped<IApplicationHandler, ApplicationHandler>();
            services.AddScoped<IAirportHandler, AirportHandler>();
            services.AddScoped<ICarHandler, CarHandler>();
            services.AddScoped<ICurrencyHandler, CurrencyHandler>();
            services.AddScoped<IExpenseHandler, ExpenseHandler>();
            services.AddScoped<IExpenseTypeHandler, ExpenseTypeHandler>();
            services.AddScoped<ICarHandler, CarHandler>();
            services.AddScoped<IIncomeHandler, IncomeHandler>();
            services.AddScoped<IMovieHandler, MovieHandler>();
            services.AddScoped<IPoiHandler, PoiHandler>();
            services.AddScoped<IProjectHandler, ProjectHandler>();
            services.AddScoped<ITaskHandler, TaskHandler>();
            services.AddScoped<ITrackingHandler, TrackingHandler>();
            services.AddScoped<IVendorHandler, VendorHandler>();


            services.AddMvc()
                    .AddXmlDataContractSerializerFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var logger = loggerFactory.CreateLogger(nameof(Startup));
            logger.LogInformation((int)LogEvent.ApiInitiated, "Started!");

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseAuthenticationMiddleware();
            app.UseMvc();          
        }
    }
}
