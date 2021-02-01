using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ProjectIvy.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Information)
                                                  .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                                                  .Enrich.FromLogContext()
                                                  .WriteTo.Console()
                                                  .WriteTo.File("./log.txt",
                                                                LogEventLevel.Debug,
                                                                fileSizeLimitBytes: 1_000_000,
                                                                rollingInterval: RollingInterval.Day,
                                                                rollOnFileSizeLimit: true,
                                                                shared: true,
                                                                flushToDiskInterval: TimeSpan.FromSeconds(15))
                                                  .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}
