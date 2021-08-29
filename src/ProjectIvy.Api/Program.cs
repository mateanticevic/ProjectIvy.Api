using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
<<<<<<< HEAD
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace ProjectIvy.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
       
            CreateHostBuilder(args).Build().Run();
=======
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

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
                                                  //.WriteTo.Graylog(new GraylogSinkOptions()
                                                  //{
                                                  //    Facility = "project-ivy-api",
                                                  //    HostnameOrAddress = "10.0.1.24",
                                                  //    Port = 12201,
                                                  //    TransportType = TransportType.Tcp
                                                  //})
                                                  .WriteTo.File("./logs/log.txt",
                                                                LogEventLevel.Debug,
                                                                fileSizeLimitBytes: 1_000_000,
                                                                rollingInterval: RollingInterval.Day,
                                                                rollOnFileSizeLimit: true,
                                                                shared: true,
                                                                flushToDiskInterval: TimeSpan.FromSeconds(15))
                                                  .CreateLogger();

            CreateHostBuilder(args).Build().Run();
>>>>>>> feef617... Image resizing support
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
