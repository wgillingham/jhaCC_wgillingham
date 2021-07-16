using jhaCC.Models;
using jhaCC.Services.Distiller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace jhaCC.Test
{
    // tests for IMessageDistiller - MessageDistillerTwitter in particular (test data would have to line up
    // with the implementation in this case (twitter json) - would be nice it abstract the test data too)
    public class MessageDistillerTests
    {
        private readonly IMessageDistiller _sut;
        private readonly FeedDataBase feedDataTwitter;

        public MessageDistillerTests()
        {


            // I struggled getting things configured - as everything is set up
            // to have required object injected...
            // ended up copying much of the code from the console app.
            // this is something I need to concentrate on.

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
                    services.AddSingleton<IMessageDistiller, MessageDistillerTwitter>();
                    services.AddSingleton<FeedDataBase, FeedDataTwitter>();
                })
                .UseSerilog()
                .Build();

            Log.Logger.Information("Start Run");


            _sut = host.Services.GetService<IMessageDistiller>();
            feedDataTwitter = host.Services.GetService<FeedDataBase>();
        }



        // note that the data is reset each time ... emojis/hashtags, etc.
        // would like to do another test where the data/numbers are accumulated... but, 
        // as it is, we are just checkig the processing of single messages.
        [Theory]
        [ClassData(typeof(DistillTwitterMessageTestData))]
        public void DistillTwitterMessageResultsVerified(DistillTwitterMessageTestData testData) 
        {
            
            feedDataTwitter.ResetData();
            KeyValuePair<string, int> keyValuePair;

            _sut.DistillMessageAsync(testData.rawJSON);

            // unique Emoji
            Assert.Equal(testData.expectedUniqueEmojiCount, feedDataTwitter.UniqueEmojiCount);

            // our 'top' emoji
            keyValuePair = feedDataTwitter.TopEmoji.DefaultIfEmpty<KeyValuePair<string, int>>(new KeyValuePair<string, int>("", 0)).FirstOrDefault();
            Assert.Equal(testData.expectedTopEmoji, keyValuePair.Key);


            // unique Hashtag
            Assert.Equal(testData.expectedUniqueHashtagCount, feedDataTwitter.UniqueHashtagCount);

            // our 'top' Hashtag
            keyValuePair = feedDataTwitter.TopHashtag.DefaultIfEmpty<KeyValuePair<string, int>>(new KeyValuePair<string, int>("", 0)).FirstOrDefault();
            Assert.Equal(testData.expectedTopHashTag, keyValuePair.Key);


            // unique Domain
            Assert.Equal(testData.expectedUniqueDomainCount, feedDataTwitter.UniqueDomainCount);

            // our 'top' Domain
            keyValuePair = feedDataTwitter.TopDomains.DefaultIfEmpty<KeyValuePair<string, int>>(new KeyValuePair<string, int>("", 0)).FirstOrDefault();
            Assert.Equal(testData.expectedTopDomain, keyValuePair.Key);
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
