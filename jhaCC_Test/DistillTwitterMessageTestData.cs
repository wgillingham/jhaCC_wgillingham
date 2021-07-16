using System.Collections;
using System.Collections.Generic;

namespace jhaCC.Test
{
    // a way to get at lost of complex data... could drive from db
    // currently 6 hard-coded simple cases
    public class DistillTwitterMessageTestData : IEnumerable<object[]>
    {
        public string rawJSON;
        public int expectedUniqueEmojiCount;
        public string expectedTopEmoji;
        public int expectedUniqueHashtagCount;
        public string expectedTopHashTag;
        public int expectedUniqueDomainCount;
        public string expectedTopDomain;


        // used by test [ClassData]
        public DistillTwitterMessageTestData()
        {
        }

        // constructor used by IEnumerable<object[]> 
        public DistillTwitterMessageTestData(string rawJSON, int expectedUniqueEmojiCount, string expectedTopEmoji, int expectedUniqueHashtagCount, string expectedTopHashTag, int expectedUniqueDomainCount, string expectedTopDomain)
        {
            this.rawJSON = rawJSON;
            this.expectedUniqueEmojiCount = expectedUniqueEmojiCount;
            this.expectedTopEmoji = expectedTopEmoji;
            this.expectedUniqueHashtagCount = expectedUniqueHashtagCount;
            this.expectedTopHashTag = expectedTopHashTag;
            this.expectedUniqueDomainCount = expectedUniqueDomainCount;
            this.expectedTopDomain = expectedTopDomain;
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            yield return new object[]
            {
                new DistillTwitterMessageTestData
                (
                    "{\"data\":{\"attachments\":{},\"created_at\":\"2021-07-16T08:43:22.000Z\",\"entities\":{\"mentions\":[{\"start\":0,\"end\":14,\"username\":\"The_MawuFemor\",\"id\":\"2254667811\"}]},\"id\":\"1415955204802064384\",\"text\":\"@The_MawuFemor Yassss😩😩😩😩\"}}",
                    1,
                    "😩",
                    0,
                    "",
                    0,
                    ""
                )
            };
            yield return new object[]
                 {
                    new DistillTwitterMessageTestData
                    (
                        "{\"data\":{\"attachments\":{},\"created_at\":\"2021-07-16T08:43:22.000Z\",\"entities\":{\"hashtags\":[{\"start\":64,\"end\":69,\"tag\":\"BGYO\"}],\"mentions\":[{\"start\":3,\"end\":18,\"username\":\"DimpleniGelo04\",\"id\":\"1390519125202538496\"},{\"start\":70,\"end\":78,\"username\":\"bgyo_ph\",\"id\":\"1237243500967555072\"}]},\"id\":\"1415955204831203331\",\"text\":\"RT @DimpleniGelo04: Pag ito hopia naku naku hahah\n\nBGYO IS BACK\n#BGYO @bgyo_ph\"}}",
                        0,
                        "",
                        1,
                        "BGYO",
                        0,
                        ""
                    )
                };
            yield return new object[]
                 {
                    new DistillTwitterMessageTestData
                    (
                        "{\"data\":{\"attachments\":{},\"created_at\":\"2021-07-16T08:43:22.000Z\",\"entities\":{\"hashtags\":[{\"start\":73,\"end\":78,\"tag\":\"BGYO\"}],\"mentions\":[{\"start\":3,\"end\":18,\"username\":\"jszstanbinigyo\",\"id\":\"1117702231346671617\"},{\"start\":79,\"end\":87,\"username\":\"bgyo_ph\",\"id\":\"1237243500967555072\"}]},\"id\":\"1415955204839669766\",\"text\":\"RT @jszstanbinigyo: Wait, imma cry first if this is legit\n\nBGYO IS BACK \n#BGYO @bgyo_ph\"}}",
                        0,
                        "",
                        1,
                        "BGYO",
                        0,
                        ""
                    )
                };
            yield return new object[]
                 {
                    new DistillTwitterMessageTestData
                    (
                        "{\"data\":{\"attachments\":{},\"created_at\":\"2021-07-16T08:43:26.000Z\",\"entities\":{},\"id\":\"1415955221616893953\",\"text\":\"renting suck sucks.....\"}}",
                        0,
                        "",
                        0,
                        "",
                        0,
                        ""
                    )
                };
            yield return new object[]
                 {
                    new DistillTwitterMessageTestData
                    (
                        "{\"data\":{\"attachments\":{},\"created_at\":\"2021-07-16T08:43:27.000Z\",\"entities\":{\"mentions\":[{\"start\":0,\"end\":10,\"username\":\"B_Shahian\",\"id\":\"1213913072403828737\"}]},\"id\":\"1415955225786175488\",\"text\":\"@B_Shahian 😂😂😂\"}}",
                        1,
                        "😂",
                        0,
                        "",
                        0,
                        ""
                    )
                };
            yield return new object[]
                 {
                    new DistillTwitterMessageTestData
                    (
                        "{\"data\":{\"attachments\":{\"media_keys\":[\"3_1415954281048559616\",\"3_1415954286941507584\"]},\"created_at\":\"2021-07-16T08:43:26.000Z\",\"entities\":{\"mentions\":[{\"start\":3,\"end\":10,\"username\":\"HutCat\",\"id\":\"1362257404159815680\"}],\"urls\":[{\"start\":12,\"end\":35,\"url\":\"https://t.co/lresdQGIPE\",\"expanded_url\":\"https://twitter.com/HutCat/status/1415954289491652610/photo/1\",\"display_url\":\"pic.twitter.com/lresdQGIPE\"},{\"start\":12,\"end\":35,\"url\":\"https://t.co/lresdQGIPE\",\"expanded_url\":\"https://twitter.com/HutCat/status/1415954289491652610/photo/1\",\"display_url\":\"pic.twitter.com/lresdQGIPE\"}]},\"id\":\"1415955221579063300\",\"text\":\"RT @HutCat: https://t.co/lresdQGIPE\"},\"includes\":{\"media\":[{\"media_key\":\"3_1415954281048559616\",\"type\":\"photo\"},{\"media_key\":\"3_1415954286941507584\",\"type\":\"photo\"}]}}",
                        0,
                        "",
                        0,
                        "",
                        1,
                        "TWITTER.COM"
                    )
                };
        }


    }
}
