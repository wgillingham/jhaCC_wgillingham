using jhaCC.Models;
using jhaCC.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jhaCC.Services.Distiller
{
    // implementation of IMessageDistiller for Twitter feed
    public class MessageDistillerTwitter : IMessageDistiller
    {
        private readonly ILogger<MessageDistillerTwitter> log;
        private readonly FeedDataBase feedData;

        private readonly string emojiRegEx;
        private readonly Regex reg;

        public MessageDistillerTwitter(ILogger<MessageDistillerTwitter> log, FeedDataBase feedData)
        {
            this.log = log;
            this.feedData = feedData;

            // we use a regex string to find/count emojis ( (c) Felix König https://github.com/Felk/UnicodeEmojiRegex )
            // load and compile it once
            emojiRegEx = File.ReadAllText(@".\resources\emojiRegEx.txt");
            reg = new Regex(emojiRegEx, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        }

        public async Task ConsumeMessageAsync(MessageDetails messageDetails)
        {
            await feedData.ConsumeMessageDetails(messageDetails);
        }

        public async Task DistillMessageAsync(string rawMessage)
        {
            // we expect rawMessage to be json text
            MessageDetails messageDetails;

            if (rawMessage.Length > 0)
            {
                try
                {
                    JObject jRoot = Newtonsoft.Json.Linq.JObject.Parse(rawMessage);

                    messageDetails = new MessageDetails
                    {
                        MessageSource = "Twitter",
                        MessageID = (string)jRoot.SelectToken("data.id"),
                        Timestamp = Convert.ToDateTime((string)jRoot.SelectToken("data.created_at"))
                    };

                    MatchCollection matches = reg.Matches((string)jRoot.SelectToken("data.text"));
                    if (matches.Count() > 0)
                    {
                        var distinctMatches = matches.OfType<Match>().GroupBy(x => x.Value).Select(x => x.First()).ToList();

                        foreach (Match match in distinctMatches)
                        {
                            messageDetails.AddEmoji(match.Value);
                        }
                    }

                    var hashtags = jRoot.SelectTokens("data.entities.hashtags[*]").Select(s => (string)s["tag"]);
                    foreach (string hashtag in hashtags)
                    {
                        messageDetails.AddHashtag(hashtag);
                    }

                    var urls = jRoot.SelectTokens("data.entities.urls[*]").Select(s => (string)s["expanded_url"]);
                    foreach (string url in urls)
                    {
                        messageDetails.AddDomain(URLHelpers.GetDomainNameOfUrlString(url));
                    }

                    messageDetails.ContainsImageURLs = jRoot.SelectTokens("includes.media[*]").Any(s => (string)s["type"] == "photo");

                    // we consume the message at this time...
                    await this.ConsumeMessageAsync(messageDetails);
                }
                catch (Exception)
                {
                    // AddRejectedMessage will still log a warning (zero length will be reported)
                    feedData.AddRejectedMessage(rawMessage);
                }
            }
            else
            {
                feedData.AddRejectedMessage(rawMessage);
            }
        }
    }

}