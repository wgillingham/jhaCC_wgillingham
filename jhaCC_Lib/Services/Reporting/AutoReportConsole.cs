using jhaCC.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jhaCC.Services.Reporting
{
    // implementation of IAutoReport to report to the console.  could have been nicer.
    public class AutoReportConsole : IAutoReport
    {
        private readonly IConfiguration config;
        private readonly FeedDataBase feedData;

        public AutoReportConsole(IConfiguration config, FeedDataBase feedData)
        {
            this.config = config;
            this.feedData = feedData;
        }

        // should re-write using template
        public Task RenderReport()
        {
            IEnumerable<KeyValuePair<string, int>> TopValue;

            Console.WriteLine($"Feed Data Dump @{DateTime.Now}");

            Console.WriteLine($"\tTotal Messages:\t{feedData.TotalMessages}");
            Console.WriteLine($"\tTotal MessagesRejected:\t{feedData.TotalMessagesRejected}");

            Console.WriteLine($"\tAverage Messages Per Second:\t{feedData.MessagesPerSecond}");
            Console.WriteLine($"\tAverage Messages Per Minute:\t{feedData.MessagesPerMinute}");
            Console.WriteLine($"\tAverage Messages Per Hour:\t{feedData.MessagesPerHour}");
            Console.WriteLine($"\tAverage Messages Per Day:\t{feedData.MessagesPerDay}");

            Console.WriteLine($"\tTotal With Emoji:\t{feedData.TotalWithEmoji}");
            Console.WriteLine($"\t\tPercent With Emoji:\t{feedData.PercentWithEmoji}%");
            Console.WriteLine($"\t\tUnique Emojis:\t{feedData.UniqueEmojiCount}");
            TopValue = feedData.TopEmoji;

            if (TopValue.Count() > 0)
            {
                Console.WriteLine($"\t\tTop {config.GetValue<string>("Services:Reporting:AutoReportTopCount")} Emojis:");

                foreach (KeyValuePair<string, int> topvalue in TopValue)
                {
                    Console.WriteLine($"\t\t\t{topvalue.Key}\t{topvalue.Value} time(s)");
                }
            }

            Console.WriteLine($"\tTotal With Hashtag:\t{feedData.TotalWithHashtag}");
            Console.WriteLine($"\t\tUnique Hashtags:\t{feedData.UniqueHashtagCount}");
            TopValue = feedData.TopHashtag;
            if (TopValue.Count() > 0)
            {
                Console.WriteLine($"\t\tTop {config.GetValue<string>("Services:Reporting:AutoReportTopCount")} Hashtags:");

                foreach (KeyValuePair<string, int> topvalue in TopValue)
                {
                    Console.WriteLine($"\t\t\t{topvalue.Key}\t{topvalue.Value} time(s)");
                }
            }

            Console.WriteLine($"\tTotal With Image:\t{feedData.TotalWithImage}");
            Console.WriteLine($"\t\tPercent With Image:\t{feedData.PercentWithImage}%");

            Console.WriteLine($"\tTotal With URL:\t{feedData.TotalWithURL}");
            Console.WriteLine($"\t\tPercent With URL:\t{feedData.PercentWithURL}%");
            Console.WriteLine($"\t\tUnique Domains:\t{feedData.UniqueDomainCount}");
            TopValue = feedData.TopDomains;
            if (TopValue.Count() > 0)
            {
                Console.WriteLine($"\t\tTop {config.GetValue<string>("Services:Reporting:AutoReportTopCount")} Domains:");

                foreach (KeyValuePair<string, int> topvalue in TopValue)
                {
                    Console.WriteLine($"\t\t\t{topvalue.Key}\t{topvalue.Value} time(s)");
                }
            }

            Console.WriteLine();

            return Task.FromResult(0);
        }
    }
}