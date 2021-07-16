using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace jhaCC.Services.Monitor
{
    // wrapper/BackgroundService - wraps a call to the IMessageMonitor implementation calling RetrieveMessagesAsync
    public class MessageMonitorBackgroundService : BackgroundService
    {
        private readonly ILogger<MessageMonitorBackgroundService> log;
        private readonly IConfiguration config;
        private readonly IMessageMonitor messageMonitor;

        private DateTime lastError = default;

        public MessageMonitorBackgroundService(ILogger<MessageMonitorBackgroundService> log, IConfiguration config, IMessageMonitor messageMonitor)
        {
            this.log = log;
            this.config = config;
            this.messageMonitor = messageMonitor;
        }
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await messageMonitor.RetrieveMessagesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // at what point do we give up trying?
                    // for now - log the exception, and let the 'while' loop attempt to reconnect
                    log.LogError(ex.Message);
                    log.LogError(ex.StackTrace);

                    var interval = DateTime.Now - lastError;
                    var reconnectDelay = config.GetValue<int>("Services:Monitor:ReconnectDelaySeconds", 15);

                    // we will pause if this is a recent error, otherwise no pause, and let while loop continue
                    // we could track the time we were down and use 'backfill_minutes' parameter to capture (some) lost data
                    // (but, we would need to look out for dupes, and I have none of the groundwork set out for that... but,
                    // storing 10minutes of tweet id's might do the trick)
                    if (interval.TotalSeconds < reconnectDelay)
                    {
                        log.LogWarning($"Connection issue - Pausing { reconnectDelay } seconds");
                        // we will pause some seconds
                        await Task.Delay(reconnectDelay * 1000);
                    }

                    lastError = DateTime.Now;
                }

            }
        }
    }
}