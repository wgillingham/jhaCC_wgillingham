using jhaCC.Models;
using jhaCC.Services.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace jhaCC.Reporting
{
    // background service - simple http listerner for simple report (json serialized version of feedData)
    // could be webservice - but would want to make seperate project for that
    public class ReportListenerBackgroundService : BackgroundService
    {
        private readonly FeedDataBase feedData;

        private static HttpListener Listener;

        public ReportListenerBackgroundService(FeedDataBase feedData, ILogger<AutoReportBackgroundService> log, IConfiguration config)
        {
            this.feedData = feedData;

            Listener = new HttpListener { Prefixes = { $"http://localhost:{ config.GetValue<int>("Services:Reporting:ReportListenerPort", 8888) }/" } };
            Listener.Start();

            log.LogInformation($"For instant stats, visit http://localhost:{ config.GetValue<int>("Services:Reporting:ReportListenerPort", 8888) }/");
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
                    //GetContextAsync() returns when a new request come in
                    var context = await Listener.GetContextAsync();
                    lock (Listener)
                    {
                        ProcessRequest(context, feedData);
                    }
                }
                catch (Exception e)
                {
                    if (e is HttpListenerException) return;
                }
            }
        }

        private static void ProcessRequest(HttpListenerContext context, FeedDataBase feedData)
        {
            using var response = context.Response;
            try
            {
                var handled = false;
                switch (context.Request.Url.AbsolutePath)
                {
                    case "/":
                        switch (context.Request.HttpMethod)
                        {
                            case "GET":
                                response.ContentType = "application/json";

                                var responseBody = JsonConvert.SerializeObject(feedData);

                                //Write to the response stream
                                var buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
                                response.ContentLength64 = buffer.Length;
                                response.OutputStream.Write(buffer, 0, buffer.Length);
                                handled = true;
                                break;
                        }
                        break;
                }
                if (!handled)
                {
                    response.StatusCode = 404;
                }
            }
            catch (Exception e)
            {
                //Return the exception details the client - you may or may not want to do this
                response.StatusCode = 500;
                response.ContentType = "application/json";
                var buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}