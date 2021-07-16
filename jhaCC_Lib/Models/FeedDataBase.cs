using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jhaCC.Models
{
    // base class for feedData...
    // (was an interface - but really only needed to override ConsumeMessageDetails)
    public abstract class FeedDataBase
    {

        // helper enum for 'MessagesPerXXX'
        public enum ReportInterval
        {
            seconds = 0,
            minutes = 1,
            hours = 2,
            days = 3,
            total = 99
        }

        private readonly ILogger<FeedDataBase> log;

        private DateTime startDateTime = default;

        private int totalMessages = 0;
        private int totalMessagesRejected = 0;

        private int totalWithURL = 0;
        private int totalWithHashtag = 0;
        private int totalWithImage = 0;
        private int totalWithEmoji = 0;

        private KeyAccumulator hashtags;
        private KeyAccumulator emojis;
        private KeyAccumulator domains;


        public void ResetData()
        {
            // mainly added to simplify testing
            totalMessages = 0;
            totalMessagesRejected = 0;

            totalWithURL = 0;
            totalWithHashtag = 0;
            totalWithImage = 0;
            totalWithEmoji = 0;

            hashtags.Clear();
            emojis.Clear();
            domains.Clear();
    }

        public FeedDataBase(ILogger<FeedDataBase> log, IConfiguration config)
        {
            this.log = log;

            // the start time is a little off in that it is set a bit before processing begins...
            hashtags = new KeyAccumulator(config);
            emojis = new KeyAccumulator(config);
            domains = new KeyAccumulator(config);
        }


        //public ILogger<FeedDataBase> Log { get => log; }
        //public IConfiguration Config { get => config; }

        //  total & totals per
        public int TotalMessages
        {
            get => totalMessages;
            set
            {
                if (startDateTime == default)
                {
                    startDateTime = DateTime.Now;
                }

                totalMessages = value;
            }
        }

        public double MessagesPerSecond { get => MessagesPer(ReportInterval.seconds); }
        public double MessagesPerMinute { get => MessagesPer(ReportInterval.minutes); }
        public double MessagesPerHour { get => MessagesPer(ReportInterval.hours); }
        public double MessagesPerDay { get => MessagesPer(ReportInterval.days); }


        //Average tweets per hour/minute/second 
        //      MessagesPer()
        public double MessagesPer(ReportInterval reportInterval)
        {
            TimeSpan timeSpan = DateTime.Now - startDateTime;

            if (this.TotalMessages > 0)
            {
                Double intervals = reportInterval switch
                {
                    ReportInterval.days => timeSpan.TotalDays,
                    ReportInterval.hours => timeSpan.TotalHours,
                    ReportInterval.seconds => timeSpan.TotalSeconds,
                    ReportInterval.minutes => timeSpan.TotalMinutes,
                    ReportInterval.total => timeSpan.TotalSeconds,
                    _ => 1
                };

                return (intervals == 0) ? 0 : this.TotalMessages / intervals;
            }
            else
            {
                return 0;
            }
        }

        // emoji tracking
        public KeyAccumulator Emojis { get => emojis; }
        public int TotalWithEmoji { get => totalWithEmoji; set => totalWithEmoji = value; }
        public int UniqueEmojiCount { get => emojis.Count(); }
        public double PercentWithEmoji { get => (TotalMessages == 0) ? 0 : totalWithEmoji / (double)TotalMessages; }
        public IEnumerable<KeyValuePair<string, int>> TopEmoji { get => emojis.GetTopValue(); }


        // hashtag tracking
        public KeyAccumulator Hashtags { get => hashtags; }
        public IEnumerable<KeyValuePair<string, int>> TopHashtag { get => hashtags.GetTopValue(); }
        public int TotalWithHashtag { get => totalWithHashtag; set => totalWithHashtag = value; }
        public int UniqueHashtagCount { get => hashtags.Count(); }

        // image tracking
        public int TotalWithImage { get => totalWithImage; set => totalWithImage = value; }
        public double PercentWithImage { get => (TotalMessages == 0) ? 0 : totalWithImage / (double)TotalMessages; }


        // domain (url) tracking
        public KeyAccumulator Domains { get => domains; }
        public int TotalWithURL { get => totalWithURL; set => totalWithURL = value; }
        public double PercentWithURL { get => (TotalMessages == 0) ? 0 : totalWithURL / (double)TotalMessages; }
        public int UniqueDomainCount { get => domains.Count(); }
        public IEnumerable<KeyValuePair<string, int>> TopDomains { get => domains.GetTopValue(); }


        // other
        public int TotalMessagesRejected { get => totalMessagesRejected; set => totalMessagesRejected = value; }
        public void AddRejectedMessage(string RawText)
        {
            totalMessagesRejected++;

            log.LogWarning($"Message Rejected ({ RawText.Length } characters): { RawText }");
        }

        public abstract Task ConsumeMessageDetails(MessageDetails messageDetails);

    }
}