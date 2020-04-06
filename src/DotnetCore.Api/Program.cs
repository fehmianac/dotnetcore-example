using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace DotnetCore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "dotnetcore-api")
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.WithProperty("Environment", env);


            if (env != null && env.Equals("Production"))
            {
                loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
                loggerConfiguration.WriteTo.File(new CompactJsonFormatter(), "logs/log.txt", rollingInterval: RollingInterval.Day);
            }

            if (env != null && env.Equals("Development"))
            {
                loggerConfiguration.WriteTo.Console();
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            try
            {
                Log.Information("dotnetcore-api starting up.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "dotnetcore-api failed to start.");
            }
            finally
            {
                Log.Information("dotnetcore-api shutting down.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        private static IConfigurationRoot GetConfigurationRoot(string[] args, string env)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();
        }
    }
}