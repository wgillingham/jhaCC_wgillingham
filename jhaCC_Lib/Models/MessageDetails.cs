using System;
using System.Collections.Generic;

namespace jhaCC.Models
{
    public class MessageDetails
    {
        private readonly List<string> hashtags = new List<String>();
        private readonly List<string> domains = new List<String>();
        private readonly List<string> emojis = new List<String>();

        private DateTime timestamp;

        bool containsImageURLs = false;

        private string messageSource;
        private string messageID;

        public void AddHashtag(string hashtag)
        {
            hashtag = hashtag.ToLower();

            if (!hashtags.Contains(hashtag))
            {
                hashtags.Add(hashtag);
            }
        }

        public void AddDomain(string domain)
        {
            domain = domain.ToLower();

            if (!this.domains.Contains(domain))
            {
                this.domains.Add(domain);
            }
        }

        public void AddEmoji(string emoji)
        {
            if (!emojis.Contains(emoji))
            {
                emojis.Add(emoji);
            }
        }

        public List<String> Hashtags { get => hashtags; }
        public List<String> Domains { get => domains; }
        public List<String> Emojis { get => emojis; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public bool ContainsImageURLs { get => containsImageURLs; set => containsImageURLs = value; }

        // Message source - i.e. Twitter
        public string MessageSource { get => messageSource; set => messageSource = value; }

        public string MessageID { get => messageID; set => messageID = value; }
    }
}
