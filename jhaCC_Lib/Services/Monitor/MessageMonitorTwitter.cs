using jhaCC.Services.Distiller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jhaCC.Services.Monitor
{
    // implementation of IMessageMonitor for Twitter
    public class MessageMonitorTwitter : IMessageMonitor
    {
        private readonly ILogger<MessageMonitorTwitter> log;
        private readonly IConfiguration config;
        private readonly IMessageDistiller messageDistiller;
        private readonly Guid guid = Guid.NewGuid();

        private readonly string bearerToken = "";

        public MessageMonitorTwitter(ILogger<MessageMonitorTwitter> log, IConfiguration config, IMessageDistiller messageDistiller)
        {
            this.log = log;
            this.config = config;
            this.messageDistiller = messageDistiller;

            bearerToken = config.GetValue<string>("Services:Monitor:MessageMonitorTwitter:TwitterBearerToken");
        }

        public async Task RetrieveMessagesAsync(CancellationToken cancellationToken)
        {
            var endpoint = config.GetValue<string>("Services:Monitor:MessageMonitorTwitter:FeedEndpoint", "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at,entities&expansions=attachments.media_keys");

            log.LogInformation("feed initialization...");

            while (!cancellationToken.IsCancellationRequested)
            {
                // for testing and grabbing test data
                //using StreamWriter writer = new StreamWriter("sampleOutput.txt", false, Encoding.UTF8);

                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                request.Headers.Add("Accept", "application/json");

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                using var body = await response.Content.ReadAsStreamAsync();
                using var reader = new System.IO.StreamReader(body);

                while (!reader.EndOfStream)
                {
                    try
                    {
                        var rawMessageJSON = await reader.ReadLineAsync();

                        // to grab raw test data
                        //writer.WriteLine(rawMessageJSON);

                        log.LogDebug($"MessageMonitor { guid } - Message: { rawMessageJSON }");


                        // this might be a mistake... I was to run and forget this task...
                        // more research needed for best approach/wisdom check
                        Task.Run(() => messageDistiller.DistillMessageAsync(rawMessageJSON));

                    }
                    catch
                    {
                        // I didn't want to include these since I am using 'using' keyword... but,
                        // upon exceptions, it was obvious (upon reconnect) that a connection was
                        // sticking.  More debugging/research is needed here
                        reader.Dispose();
                        body.Dispose();
                        response.Dispose();
                        request.Dispose();
                        client.Dispose();

                        // bubble up the exception
                        throw;
                    }
                }
            }
        }

    }
}