# A Code Exercise using .Net Core 3.1 and Twiter API

In a nutshell, gather some stats from a sample of the twitter stream via the [Sampled stream API](https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/introduction).

## Important

You must supply you own [twitter bearer token](https://developer.twitter.com/en/docs/authentication/oauth-2-0).

Your twitter bearer token must be placed in appsettings.json (/jhaCC_Console/appsettings.json or appsettings.DEVELOPMENT.json)

```
      "Services": {
        "Monitor": {
          "MessageMonitorTwitter": {
            "TwitterBearerToken": "*** YOUR TOKEN HERE ***"
          }
        }
      }
```

## Solution file (jhaCC_wgillingham.sln) contains three project

### jhaCC_Lib - Library of objects required to digest twitter feed
- Models
  - FeedDataBase.cs - Base class to contain data gleened from feed
  - FeedDataTwitter.cs - Inherits from FeedDataBase.cs and overrides ConsumeMessageDetails - which accepts MessageDetails.  We accumulate based on this data
  - KeyAccumulator.cs - wrapper arround ConcurrentDictionary that allows us to store values/counts (used for stats on emojis, hashtags, urls)
  - MessageDetails.cs - Container object for what was gleened from a single message

- Resources
  - emojiRegEx.txt - a regex string from https://github.com/Felk/UnicodeEmojiRegex and is Copyright (c) 2020 Felix König.  I simply generated and used the regex string from the aformentioned project.  The project could be incorportated directly into an application to create the regex string on demand.
- Services
  - Distiller
    - _IMessageDistiller.cs_ - interface exposing method DistillMessageAsync and ConsumeMessageAsync.
    - _IMessageDistillerTwitter.cs_ - implentation of IMessageDistiller for twitter feed
  - Monitor
    - _IMessageMonitor.cs_ - interface exposing method RetrieveMessagesAsync - used to retrieve messages in whatever manner you see fit
    - _MessageMonitorBackgroundService.cs_ - BackgroundService.  Expects IMessageMonitor instance injected and will process via RetrieveMessagesAsync()
    - _MessageMonitorTwitter.cs_ - implentation of IMessageMonitor.  streams https://api.twitter.com/2/tweets/sample/stream and processes messages via injected IMessageDistiller instance
  - Reporting
    - _AutoReportBackgroundService.cs_ - BackgroundService.  Executes injected implentation of IAutoReport (calls its RenderReport method on a schedule)
    - _AutoReportConsole.cs_ - Implentation of IAutoReport.  Renders stats from injected FeedDataBase to the console (RenderReport())
    - _IAutoReport.cs_ - interface exposing RenderReport method.  Expected to render a report on a schedule (i.e. ever 20 seconds)
    - _ReportListenerBackgroundService.cs_ - BackgroundService.  Cheap report via http://localhost:8888 (configured via appsettings.json)

### jhaCC_Console - this is your best option to execute as a demo
- _Program.cs_ - References jhaCC_Lib.  ConfigureServices and runs the host.  See console for output.  Also http://localhost:8888 (by default) for 2% fancier output.

### jhaCC_Test (XUnit implentation)
- _DistillTwitterMessageTestData.cs_ - testdata container for MessageDistillerTests - for use as [ClassData] exposed data
- _MessageDistillerTests.cs_ - simple test case example.  6 instances of DistillTwitterMessageTestData are individually sent to DistillTwitterMessageResultsVerified.  Simple test to count number of emojis/hashtags/domains expected and found in each message
