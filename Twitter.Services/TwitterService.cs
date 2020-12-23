using Microsoft.Extensions.Options; 
using System.Collections.Generic; 
using System.Threading.Tasks;
using Tweetinvi.Models;   
using System;
using System.Linq; 
using Tweetinvi;
using Microsoft.Extensions.Caching.Memory; 
using Tweetinvi.Models.V2;
using TwitterServices.Interfaces;
using TwitterModels;
using TwitterServices.Helpers;
using System.Net;
using System.Diagnostics;

namespace TwitterServices
{
    public class TwitterService : ITwitterService
    {
        private readonly AppSettings _settings;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "CACHE_KEY_TWITTER";

        public TwitterService(IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            _settings = appSettings.Value;
            _cache = memoryCache;
        }

        private List<TweetV2> GetTweets()
        {
            var isFound = _cache.TryGetValue(_cacheKey, out List<TweetV2> tweets);
            if (isFound)
                return tweets;

            return new List<TweetV2>();
        }

        private void SaveTweet(TweetV2 tweet)
        {
            var tweets = GetTweets();
            tweets.Add(tweet); 
            _cache.Set(_cacheKey, tweets, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(20)).SetAbsoluteExpiration(TimeSpan.FromHours(20)));
        }

        private async Task<TwitterClient> ConnectToTwitter()
        {
            var consumerOnlyCredentials = new ConsumerOnlyCredentials(_settings.TWITTER_CONSUMER_KEY, _settings.TWITTER_CONSUMER_SECRET);
            var appClientWithoutBearer = new TwitterClient(consumerOnlyCredentials);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            var bearerToken = await appClientWithoutBearer.Auth.CreateBearerTokenAsync();
            var appCredentials = new ConsumerOnlyCredentials(_settings.TWITTER_CONSUMER_KEY, _settings.TWITTER_CONSUMER_SECRET)
            {
                BearerToken = bearerToken
            };

            return new TwitterClient(appCredentials);
        }
         
        public TwitterStatModel GetSampledStreamStats()
        {
            var result = new TwitterStatModel();
            var tweets = GetTweets();
            if (!tweets.Any())
            {
                Task.Run(() => ProcessTweets());
                //throw new Exception("No tweets found in memory. Please run ProcessTweets first");
                //return result;
            }

            var tweetCount = tweets.Count;
            var hashTags = new List<string>();
            var domains = new List<string>();
            var photoUrls = new List<string>();
            var emojis = new List<string>();
            var firstTweetDate = DateTime.MaxValue;
            foreach (var item in tweets.ToList())
            {
                if (result.LastTweetDate < item.CreatedAt.UtcDateTime)
                    result.LastTweetDate = item.CreatedAt.UtcDateTime;

                if (firstTweetDate > item.CreatedAt.UtcDateTime)
                    firstTweetDate = item.CreatedAt.UtcDateTime;

                if(item.Entities == null)
                    continue;

                if (item.Entities.Hashtags != null && item.Entities.Hashtags.Any())
                {
                    foreach (var tag in item.Entities.Hashtags)
                    {
                        if (!string.IsNullOrEmpty(tag.Tag))
                            hashTags.Add(tag.Tag);
                    }
                }

                if (item.Entities.Urls != null && item.Entities.Urls.Any())
                {
                    foreach (var url in item.Entities.Urls)
                    {
                        if (!string.IsNullOrEmpty(url.ExpandedUrl))
                        {
                            var uri = new Uri(url.ExpandedUrl);
                            domains.Add(uri.Host);

                            if (StringHelper.IsPhoto(uri.LocalPath))
                                photoUrls.Add(uri.AbsoluteUri);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.Text))
                {
                    var emojiList = StringHelper.ParseEmojis(item.Text);
                    if (emojiList.Any())
                        emojis.AddRange(emojiList);
                }
            }

            result.TotalNumberOfTweetsReceived = tweetCount;
            var totalSecondsElapsed = (result.LastTweetDate - firstTweetDate).TotalSeconds;
            if (totalSecondsElapsed > 0)
                result.AverageTweetsPerSecond = (decimal)(tweetCount / totalSecondsElapsed);

            if (domains.Any())
            {
                result.PercentOfTweetsWithUrls = StringHelper.GetPercentage(domains, tweetCount); 
                result.TopDomain = StringHelper.GetTop(domains);
            }

            if (photoUrls.Any())
                result.PercentOfTweetsWithPhotoUrls = StringHelper.GetPercentage(photoUrls, tweetCount);

            if (hashTags.Any())
                result.TopHastag = StringHelper.GetTop(hashTags);

            if (emojis.Any())
                result.PercentOfTweetsWithEmojis = StringHelper.GetPercentage(emojis, tweetCount);
             
            return result;
        }

        public async Task<IEnumerable<string>> ProcessTweets()
        { 
            var appClient = await ConnectToTwitter();

            var tweets = new List<string>();
            var sampleStreamV2 = appClient.StreamsV2.CreateSampleStream();
            sampleStreamV2.TweetReceived += (sender, args) =>
            {
                if (args.Tweet != null)
                {
                    tweets.Add(args.Tweet.Text);
                    SaveTweet(args.Tweet);
                    Debug.WriteLine(args.Tweet.Text);
                    //publish this tweet received event w pub/sub
                }
            }; 

            await sampleStreamV2.StartAsync();
            return tweets;
        }  

    }
}
