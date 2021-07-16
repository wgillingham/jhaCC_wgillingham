using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace jhaCC.Models
{

    public class FeedDataTwitter : FeedDataBase
    {
        private readonly ILogger<FeedDataTwitter> log;

        public FeedDataTwitter(ILogger<FeedDataTwitter> log, IConfiguration config) : base(log, config)
        {
            this.log = log;
        }

        public override Task ConsumeMessageDetails(MessageDetails messageDetails)
        {
            if (messageDetails is null)
            {
                TotalMessagesRejected++;
            }
            else
            {
                // we have something valid
                TotalMessages++;

                if (messageDetails.ContainsImageURLs)
                {
                    this.TotalWithImage++;
                }

                if (messageDetails.Hashtags.Count > 0)
                {
                    this.TotalWithHashtag++;
                    Hashtags.Increment(messageDetails.Hashtags);
                }

                if (messageDetails.Emojis.Count > 0)
                {
                    this.TotalWithEmoji++;
                    Emojis.Increment(messageDetails.Emojis);
                }

                if (messageDetails.Domains.Count > 0)
                {
                    this.TotalWithURL++;
                    Domains.Increment(messageDetails.Domains);
                }
            }

            log.LogDebug($"Message { messageDetails.MessageID } consumed");
            return Task.FromResult(0);
        }

    }
}
