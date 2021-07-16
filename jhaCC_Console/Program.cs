using jhaCC.Models;
using jhaCC.Reporting;
using jhaCC.Services.Distiller;
using jhaCC.Services.Monitor;
using jhaCC.Services.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace jhaCC.Console
{
    class Program
    {
        static void Main()
        {
            var builder = new ConfigurationBuilder();

            // to allow logging prior to configuring host
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // configure host
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // add services
                    //services.AddSingleton<ITaskQueue, TaskQueue>();
                    services.AddSingleton<IMessageMonitor, MessageMonitorTwitter>();
                    services.AddSingleton<IMessageDistiller, MessageDistillerTwitter>();
                    services.AddSingleton<FeedDataBase, FeedDataTwitter>();
                    if (context.Configuration.GetValue<bool>("Services:Reporting:AutoReport"))
                    {
                        services.AddSingleton<IAutoReport, AutoReportConsole>();
                        services.AddHostedService<AutoReportBackgroundService>();
                    }
                    services.AddHostedService<ReportListenerBackgroundService>();
                    services.AddHostedService<MessageMonitorBackgroundService>();
                })
                .UseSerilog()
                .Build();

            Log.Logger.Information("Start Run");
            host.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }

    }
}