using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace jhaCC.Services.Reporting
{
    // wrapper/Background service.  Wraps implmentation of IAutoReport - calling RenderReport periodically as defined in appsettings
    public class AutoReportBackgroundService : BackgroundService
    {
        private readonly ILogger<AutoReportBackgroundService> log;
        private readonly IConfiguration config;
        private readonly IAutoReport autoReport;

        public AutoReportBackgroundService(IAutoReport autoReport, ILogger<AutoReportBackgroundService> log, IConfiguration config)
        {
            this.autoReport = autoReport;
            this.log = log;
            this.config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await BackgroundProcessingAsync(cancellationToken);
            }
        }

        private async Task BackgroundProcessingAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(config.GetValue<int>("Services:Reporting:AutoReportIntervalSeconds", 5) * 1000);
                    await autoReport.RenderReport();
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error occurred executing {autoReport}.", nameof(autoReport));
                    await Task.FromException(ex);
                }
            }
        }

    }
}
