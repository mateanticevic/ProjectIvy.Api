using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
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
                                                  .WriteTo.Graylog(new GraylogSinkOptions()
                                                  {
                                                      Facility = "project-ivy-api",
                                                      HostnameOrAddress = Environment.GetEnvironmentVariable("GRAYLOG_HOST"),
                                                      Port = Convert.ToInt32(Environment.GetEnvironmentVariable("GRAYLOG_PORT")),
                                                      TransportType = TransportType.Udp
                                                  })
                                                  .WriteTo.File("./logs/log.txt",
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
                    webBuilder.UseStartup<Startup>()
                              .UseKestrel(options =>
                              {
                                  if (Environment.GetEnvironmentVariable("USE_HTTP2") is not null)
                                    options.ConfigureEndpointDefaults(x => x.Protocols = HttpProtocols.Http2);
                              });
                })
            .UseSerilog();
    }
}
